import 'package:flutter/material.dart';
import 'package:mobile/models/reservation.dart';
import 'package:intl/intl.dart';

class ReservationTile extends StatelessWidget {
  final Reservation reservation;
  final VoidCallback? onCancel;
  final VoidCallback? onCheckIn;
  final VoidCallback? onRebook;

  const ReservationTile({
    Key? key,
    required this.reservation,
    this.onCancel,
    this.onCheckIn,
    this.onRebook,
  }) : super(key: key);

  @override
  Widget build(BuildContext context) {
    final from = DateFormat.yMd().format(reservation.from);
    final to = DateFormat.yMd().format(reservation.to);

    Icon statusIcon;
    Color statusColor;

    switch (reservation.status) {
      case 'Reserved':
        statusIcon = const Icon(Icons.event_available, color: Colors.blue);
        statusColor = Colors.blue.shade50;
        break;
      case 'CheckedIn':
        statusIcon = const Icon(Icons.check_circle, color: Colors.green);
        statusColor = Colors.green.shade50;
        break;
      case 'Cancelled':
        statusIcon = const Icon(Icons.cancel, color: Colors.red);
        statusColor = Colors.red.shade50;
        break;
      case 'Expired':
        statusIcon = const Icon(Icons.event_busy, color: Colors.grey);
        statusColor = Colors.grey.shade200;
        break;
      default:
        statusIcon = const Icon(Icons.event_note, color: Colors.orange);
        statusColor = Colors.orange.shade50;
    }

    return Card(
      color: statusColor,
      elevation: 1,
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(10)),
      margin: const EdgeInsets.symmetric(horizontal: 8, vertical: 6),
      child: Padding(
        padding: const EdgeInsets.symmetric(vertical: 12, horizontal: 16),
        child: Row(
          crossAxisAlignment: CrossAxisAlignment.center,
          children: [
            Container(
              decoration: BoxDecoration(
                color: statusColor,
                shape: BoxShape.circle,
              ),
              padding: const EdgeInsets.all(8),
              child: statusIcon,
            ),

            const SizedBox(width: 12),

            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    'Spot ${reservation.spotId}',
                    style: const TextStyle(
                      fontSize: 16,
                      fontWeight: FontWeight.w600,
                    ),
                  ),
                  const SizedBox(height: 4),
                  Text('$from → $to', style: const TextStyle(fontSize: 14)),
                  const SizedBox(height: 4),
                  Text(
                    'Status: ${reservation.status}',
                    style: TextStyle(fontSize: 13, color: Colors.grey.shade700),
                  ),
                ],
              ),
            ),

            Wrap(
              spacing: 8,
              children: [
                if (reservation.status == 'Reserved' && onCancel != null)
                  IconButton(
                    icon: const Icon(Icons.cancel, color: Colors.red),
                    onPressed: onCancel,
                    tooltip: 'Cancel',
                  ),
                if (reservation.status == 'Reserved' && onCheckIn != null)
                  IconButton(
                    icon: const Icon(Icons.check_circle, color: Colors.green),
                    onPressed: onCheckIn,
                    tooltip: 'Check‐In',
                  ),
                if (reservation.status != 'Reserved' && onRebook != null)
                  TextButton.icon(
                    onPressed: onRebook,
                    icon: const Icon(Icons.refresh, size: 18),
                    label: const Text(
                      'Re‐Book',
                      style: TextStyle(fontSize: 14),
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
