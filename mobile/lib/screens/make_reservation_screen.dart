import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:mobile/models/spot.dart';
import 'package:mobile/providers/auth_provider.dart';
import 'package:mobile/providers/reservation_provider.dart';
import 'package:mobile/providers/spot_provider.dart';
import 'package:intl/intl.dart';

class MakeReservationScreen extends StatefulWidget {
  final Spot? prefilledSpot;
  const MakeReservationScreen({Key? key, this.prefilledSpot}) : super(key: key);

  @override
  State<MakeReservationScreen> createState() => _MakeReservationScreenState();
}

class _MakeReservationScreenState extends State<MakeReservationScreen> {
  Spot? selectedSpot;
  DateTime? fromDate;
  DateTime? toDate;
  bool needsCharger = false;
  bool isSubmitting = false;

  @override
  void initState() {
    super.initState();
    if (widget.prefilledSpot != null) {
      selectedSpot = widget.prefilledSpot;
      needsCharger = selectedSpot!.capabilities.contains('ElectricCharger');
    }
  }

  Future<void> _pickDateRange() async {
    final now = DateTime.now();
    final initialRange =
        (fromDate != null && toDate != null)
            ? DateTimeRange(start: fromDate!, end: toDate!)
            : DateTimeRange(start: now, end: now.add(const Duration(days: 1)));

    final picked = await showDateRangePicker(
      context: context,
      firstDate: now,
      lastDate: now.add(const Duration(days: 365)),
      initialDateRange: initialRange,
    );
    if (picked != null) {
      setState(() {
        fromDate = picked.start;
        toDate = picked.end;
      });
    }
  }

  Future<void> _submitReservation() async {
    if (selectedSpot == null || fromDate == null || toDate == null) return;

    setState(() => isSubmitting = true);

    try {
      final auth = Provider.of<AuthProvider>(context, listen: false);
      final reservationProv = Provider.of<ReservationProvider>(
        context,
        listen: false,
      );
      final userId = auth.currentUser!.id;

      final res = await reservationProv.createReservation(
        spotId: selectedSpot!.id,
        userId: userId,
        from: fromDate!,
        to: toDate!.add(const Duration(days: 1)),
        needsCharger: needsCharger,
      );

      // Show confirmation dialog
      await showDialog(
        context: context,
        builder:
            (_) => AlertDialog(
              shape: RoundedRectangleBorder(
                borderRadius: BorderRadius.circular(12),
              ),
              title: const Text('Reservation Confirmed'),
              content: Text(
                'Your reservation (ID: ${res.id}) for spot ${selectedSpot!.key}\n'
                'from ${DateFormat.yMMMEd().format(fromDate!)} '
                'to ${DateFormat.yMMMEd().format(toDate!)} has been made.',
              ),
              actions: [
                TextButton(
                  onPressed: () {
                    Navigator.of(context).popUntil((route) => route.isFirst);
                  },
                  child: const Text('OK'),
                ),
              ],
            ),
      );
    } catch (e) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text(e.toString()),
          backgroundColor: Colors.red.shade300,
        ),
      );
    } finally {
      if (mounted) setState(() => isSubmitting = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    final spotProv = Provider.of<SpotProvider>(context);
    final allSpots = spotProv.spots ?? [];
    final theme = Theme.of(context);

    return Scaffold(
      appBar: AppBar(
        title: const Text('Make a Reservation'),
        backgroundColor: Colors.white,
        foregroundColor: theme.colorScheme.primary,
        elevation: 1,
      ),
      body: SingleChildScrollView(
        padding: const EdgeInsets.symmetric(vertical: 24, horizontal: 16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            // -- 1) Select Spot Card --
            Card(
              elevation: 2,
              shape: RoundedRectangleBorder(
                borderRadius: BorderRadius.circular(12),
              ),
              child: Padding(
                padding: const EdgeInsets.symmetric(
                  horizontal: 16,
                  vertical: 20,
                ),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.stretch,
                  children: [
                    Text(
                      '1. Choose a Spot',
                      style: theme.textTheme.titleMedium!.copyWith(
                        fontWeight: FontWeight.bold,
                      ),
                    ),
                    const SizedBox(height: 12),
                    if (widget.prefilledSpot == null) ...[
                      DropdownButtonFormField<Spot>(
                        value: selectedSpot,
                        decoration: InputDecoration(
                          labelText: 'Select Spot',
                          border: OutlineInputBorder(
                            borderRadius: BorderRadius.circular(8),
                          ),
                          filled: true,
                          fillColor: Colors.grey.shade100,
                        ),
                        items:
                            allSpots
                                .map(
                                  (s) => DropdownMenuItem(
                                    value: s,
                                    child: Text(
                                      '${s.key} '
                                      '${s.capabilities.contains('ElectricCharger') ? '⚡' : ''}',
                                    ),
                                  ),
                                )
                                .toList(),
                        onChanged: (s) {
                          setState(() {
                            selectedSpot = s;
                            needsCharger = s!.capabilities.contains(
                              'ElectricCharger',
                            );
                          });
                        },
                      ),
                    ] else ...[
                      Row(
                        children: [
                          Icon(
                            Icons.local_parking,
                            color: theme.colorScheme.primary,
                          ),
                          const SizedBox(width: 12),
                          Flexible(
                            child: Text(
                              'Spot: ${widget.prefilledSpot!.key}',
                              style: theme.textTheme.bodyLarge,
                            ),
                          ),
                        ],
                      ),
                      const SizedBox(height: 8),
                      Row(
                        children: [
                          Checkbox(value: needsCharger, onChanged: null),
                          const SizedBox(width: 4),
                          const Text('Electric Charger (fixed)'),
                        ],
                      ),
                    ],
                  ],
                ),
              ),
            ),

            const SizedBox(height: 24),

            // -- 2) Date Range Card --
            Card(
              elevation: 2,
              shape: RoundedRectangleBorder(
                borderRadius: BorderRadius.circular(12),
              ),
              child: InkWell(
                onTap: _pickDateRange,
                borderRadius: BorderRadius.circular(12),
                child: Padding(
                  padding: const EdgeInsets.symmetric(
                    horizontal: 16,
                    vertical: 20,
                  ),
                  child: Row(
                    children: [
                      Icon(
                        Icons.date_range,
                        color: theme.colorScheme.primary,
                        size: 28,
                      ),
                      const SizedBox(width: 16),
                      Expanded(
                        child: Text(
                          fromDate == null || toDate == null
                              ? 'Pick a Date Range'
                              : '${DateFormat.yMMMEd().format(fromDate!)} → '
                                  '${DateFormat.yMMMEd().format(toDate!)}',
                          style: theme.textTheme.bodyLarge!.copyWith(
                            color:
                                fromDate == null
                                    ? Colors.grey.shade600
                                    : Colors.black87,
                          ),
                        ),
                      ),
                    ],
                  ),
                ),
              ),
            ),

            const SizedBox(height: 24),

            // -- 3) Charger Option Card --
            Card(
              elevation: 2,
              shape: RoundedRectangleBorder(
                borderRadius: BorderRadius.circular(12),
              ),
              child: Padding(
                padding: const EdgeInsets.symmetric(
                  horizontal: 16,
                  vertical: 20,
                ),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.stretch,
                  children: [
                    Text(
                      '3. Options',
                      style: theme.textTheme.titleMedium!.copyWith(
                        fontWeight: FontWeight.bold,
                      ),
                    ),
                    const SizedBox(height: 12),
                    if (widget.prefilledSpot == null ||
                        (widget.prefilledSpot != null &&
                            widget.prefilledSpot!.capabilities.contains(
                              'ElectricCharger',
                            ))) ...[
                      Row(
                        children: [
                          Checkbox(
                            value: needsCharger,
                            onChanged: (val) {
                              setState(() {
                                needsCharger = val ?? false;
                              });
                            },
                          ),
                          const SizedBox(width: 4),
                          const Text('Needs Electric Charger'),
                        ],
                      ),
                    ] else ...[
                      Row(
                        children: const [
                          Icon(Icons.power_off, color: Colors.grey),
                          SizedBox(width: 12),
                          Text('No Electric Charger available'),
                        ],
                      ),
                    ],
                  ],
                ),
              ),
            ),

            const SizedBox(height: 32),

            // -- 4) Submit Button --
            isSubmitting
                ? Center(
                  child: CircularProgressIndicator(
                    color: theme.colorScheme.primary,
                  ),
                )
                : SizedBox(
                  height: 64,
                  child: ElevatedButton.icon(
                    icon: const Icon(Icons.check_circle_outline),
                    label: const Text(
                      'Reserve Now',
                      style: TextStyle(fontSize: 16),
                    ),
                    style: ElevatedButton.styleFrom(
                      shape: RoundedRectangleBorder(
                        borderRadius: BorderRadius.circular(8),
                      ),
                      backgroundColor: theme.colorScheme.primary,
                    ),
                    onPressed:
                        (selectedSpot == null ||
                                fromDate == null ||
                                toDate == null)
                            ? null
                            : _submitReservation,
                  ),
                ),
          ],
        ),
      ),
    );
  }
}
