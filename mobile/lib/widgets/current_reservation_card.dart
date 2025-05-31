import 'package:flutter/material.dart';
import 'package:mobile/models/reservation.dart';
import 'package:mobile/models/spot.dart';
import 'package:mobile/providers/spot_provider.dart';
import 'package:provider/provider.dart';
import 'package:intl/intl.dart';

class CurrentReservationCard extends StatelessWidget {
  final Reservation reservation;
  final VoidCallback onCancel;
  final VoidCallback onCheckIn;

  const CurrentReservationCard({
    Key? key,
    required this.reservation,
    required this.onCancel,
    required this.onCheckIn,
  }) : super(key: key);

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final dateFormatted = DateFormat.yMMMEd().format(reservation.from);
    final spotProv = Provider.of<SpotProvider>(context, listen: false);
    Spot? spot;
    try {
      spot = spotProv.spots!.firstWhere((s) => s.id == reservation.spotId);
    } catch (_) {
      spot = null;
    }

    Color statusColor;
    IconData statusIcon;

    switch (reservation.status) {
      case 'Reserved':
        statusColor = Colors.blue;
        statusIcon = Icons.event_available;
        break;
      case 'CheckedIn':
        statusColor = Colors.green;
        statusIcon = Icons.check_circle;
        break;
      default:
        statusColor = Colors.grey;
        statusIcon = Icons.event_note;
    }

    return Card(
      margin: const EdgeInsets.symmetric(horizontal: 16),
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
      elevation: 2,
      child: Padding(
        padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 20),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            // Top row: icon + spot key + date
            Row(
              children: [
                Icon(statusIcon, color: statusColor, size: 28),
                const SizedBox(width: 12),
                Expanded(
                  child: Text(
                    spot != null
                        ? 'Spot ${spot.key}'
                        : 'Spot ${reservation.spotId}',
                    style: theme.textTheme.titleMedium!.copyWith(
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                ),
                Text(
                  DateFormat.Hm().format(reservation.from),
                  style: theme.textTheme.bodyMedium?.copyWith(
                    color: Colors.grey.shade700,
                  ),
                ),
              ],
            ),

            const SizedBox(height: 12),

            // Date(s)
            Row(
              children: [
                Icon(
                  Icons.calendar_today,
                  size: 18,
                  color: Colors.grey.shade600,
                ),
                const SizedBox(width: 6),
                Text(
                  dateFormatted,
                  style: theme.textTheme.bodyMedium?.copyWith(
                    color: Colors.grey.shade700,
                  ),
                ),
              ],
            ),

            const SizedBox(height: 20),

            // Action buttons
            Row(
              children: [
                Expanded(
                  child: OutlinedButton.icon(
                    onPressed: onCancel,
                    icon: const Icon(Icons.cancel, color: Colors.red),
                    label: const Text(
                      'Cancel',
                      style: TextStyle(color: Colors.red),
                    ),
                    style: OutlinedButton.styleFrom(
                      side: const BorderSide(color: Colors.red),
                      shape: RoundedRectangleBorder(
                        borderRadius: BorderRadius.circular(8),
                      ),
                    ),
                  ),
                ),
                const SizedBox(width: 12),
                Expanded(
                  child: ElevatedButton.icon(
                    onPressed: onCheckIn,
                    icon: const Icon(Icons.check, color: Colors.white),
                    label: const Text('Check‐In'),
                    style: ElevatedButton.styleFrom(
                      backgroundColor: Colors.green,
                      shape: RoundedRectangleBorder(
                        borderRadius: BorderRadius.circular(8),
                      ),
                    ),
                  ),
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }
}
