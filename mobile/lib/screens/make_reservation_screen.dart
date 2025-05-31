import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:mobile/models/spot.dart';
import 'package:mobile/providers/auth_provider.dart';
import 'package:mobile/providers/reservation_provider.dart';
import 'package:mobile/models/reservation.dart';
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
    final initialDateRange =
        fromDate != null && toDate != null
            ? DateTimeRange(start: fromDate!, end: toDate!)
            : DateTimeRange(start: now, end: now.add(const Duration(days: 1)));

    final picked = await showDateRangePicker(
      context: context,
      firstDate: now,
      lastDate: now.add(const Duration(days: 365)),
      initialDateRange: initialDateRange,
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
        to: toDate!.add(const Duration(days: 1)), // backend expects exclusive?
        needsCharger: needsCharger,
      );
      // On success, show dialog and pop back
      await showDialog(
        context: context,
        builder:
            (_) => AlertDialog(
              title: const Text('Reservation Confirmed'),
              content: Text(
                'Your reservation (ID: ${res.id}) for spot ${selectedSpot!.key}\n'
                'from ${DateFormat.yMd().format(fromDate!)} to ${DateFormat.yMd().format(toDate!)} has been made.',
              ),
              actions: [
                TextButton(
                  onPressed: () {
                    Navigator.of(
                      context,
                    ).popUntil((route) => route.isFirst); // Go to Home
                  },
                  child: const Text('OK'),
                ),
              ],
            ),
      );
    } catch (e) {
      // Show a snackbar or alert dialog with error
      ScaffoldMessenger.of(
        context,
      ).showSnackBar(SnackBar(content: Text(e.toString())));
    } finally {
      setState(() => isSubmitting = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    final spotProv = Provider.of<SpotProvider>(context);
    final allSpots = spotProv.spots ?? [];
    final user = Provider.of<AuthProvider>(context).currentUser!;

    return Scaffold(
      appBar: AppBar(title: const Text('Make a Reservation')),
      body: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          children: [
            // 1. If no prefilledSpot, show a dropdown to select a spot
            if (widget.prefilledSpot == null) ...[
              DropdownButtonFormField<Spot>(
                value: selectedSpot,
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
                    needsCharger = s!.capabilities.contains('ElectricCharger');
                  });
                },
                decoration: const InputDecoration(
                  labelText: 'Select Spot',
                  border: OutlineInputBorder(),
                ),
              ),
              const SizedBox(height: 16),
            ] else ...[
              Text(
                'Spot: ${widget.prefilledSpot!.key}',
                style: const TextStyle(fontSize: 18),
              ),
              const SizedBox(height: 8),
              Row(
                children: [
                  Checkbox(value: needsCharger, onChanged: null),
                  const Text('Electric Charger (fixed)'),
                ],
              ),
              const SizedBox(height: 16),
            ],

            // 2. Date range picker
            ElevatedButton.icon(
              icon: const Icon(Icons.date_range),
              label: Text(
                fromDate == null || toDate == null
                    ? 'Pick Date Range'
                    : '${DateFormat.yMd().format(fromDate!)} → ${DateFormat.yMd().format(toDate!)}',
              ),
              onPressed: _pickDateRange,
            ),
            const SizedBox(height: 16),

            // 3. “Needs Charger” (if spot does not have one, disable)
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
                  const Text('Needs Electric Charger'),
                ],
              ),
              const SizedBox(height: 16),
            ] else ...[
              // Spot has no charger; disable the checkbox
              Row(
                children: const [
                  Checkbox(value: false, onChanged: null),
                  Text('No Electric Charger available for this spot'),
                ],
              ),
              const SizedBox(height: 16),
            ],

            // 4. Submit button
            isSubmitting
                ? const CircularProgressIndicator()
                : ElevatedButton.icon(
                  icon: const Icon(Icons.check),
                  label: const Text('Reserve'),
                  onPressed:
                      (selectedSpot == null ||
                              fromDate == null ||
                              toDate == null)
                          ? null
                          : _submitReservation,
                ),
          ],
        ),
      ),
    );
  }
}
