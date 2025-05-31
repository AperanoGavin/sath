import 'package:flutter/material.dart';
import 'package:mobile/models/reservation.dart';
import 'package:mobile/models/spot.dart';
import 'package:intl/intl.dart';

class ReservationHistoryCard extends StatelessWidget {
  final Reservation reservation;
  final Spot? spot;
  final VoidCallback onRebook;

  const ReservationHistoryCard({
    Key? key,
    required this.reservation,
    required this.spot,
    required this.onRebook,
  }) : super(key: key);

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final fromFormatted = DateFormat.yMMMd().format(reservation.from);
    final toFormatted = DateFormat.yMMMd().format(reservation.to);
    final dateRange = '$fromFormatted – $toFormatted';

    // Determine status badge color
    Color statusBg;
    Color statusText;
    IconData statusIcon;

    switch (reservation.status) {
      case 'Cancelled':
        statusBg = Colors.red.shade50;
        statusText = Colors.red.shade700;
        statusIcon = Icons.cancel;
        break;
      case 'Expired':
        statusBg = Colors.grey.shade200;
        statusText = Colors.grey.shade700;
        statusIcon = Icons.event_busy;
        break;
      case 'CheckedIn':
        statusBg = Colors.green.shade50;
        statusText = Colors.green.shade700;
        statusIcon = Icons.check_circle;
        break;
      default: // Reserved or other
        statusBg = Colors.blue.shade50;
        statusText = Colors.blue.shade700;
        statusIcon = Icons.event_available;
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
            // Top row: Spot Icon + Spot Key + Status Badge
            Row(
              children: [
                Container(
                  decoration: BoxDecoration(
                    color: statusBg,
                    shape: BoxShape.circle,
                  ),
                  padding: const EdgeInsets.all(8),
                  child: Icon(statusIcon, color: statusText, size: 24),
                ),
                const SizedBox(width: 12),
                Expanded(
                  child: Text(
                    spot != null
                        ? 'Spot ${spot!.key}'
                        : 'Spot ${reservation.spotId}',
                    style: theme.textTheme.titleMedium!.copyWith(
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                ),
                Container(
                  padding: const EdgeInsets.symmetric(
                    horizontal: 12,
                    vertical: 4,
                  ),
                  decoration: BoxDecoration(
                    color: statusBg,
                    borderRadius: BorderRadius.circular(8),
                  ),
                  child: Text(
                    reservation.status,
                    style: theme.textTheme.bodySmall?.copyWith(
                      color: statusText,
                      fontWeight: FontWeight.w600,
                    ),
                  ),
                ),
              ],
            ),

            const SizedBox(height: 12),

            // Date Range
            Row(
              crossAxisAlignment: CrossAxisAlignment.center,
              children: [
                Icon(Icons.date_range, size: 18, color: Colors.grey.shade600),
                const SizedBox(width: 6),
                Expanded(
                  child: Text(
                    dateRange,
                    style: theme.textTheme.bodyMedium?.copyWith(
                      color: Colors.grey.shade700,
                    ),
                  ),
                ),
              ],
            ),

            const SizedBox(height: 16),

            // Re‐Book Button (only for non‐Reserved states)
            Align(
              alignment: Alignment.centerRight,
              child: TextButton.icon(
                onPressed: onRebook,
                icon: const Icon(Icons.refresh, size: 18),
                label: const Text('Re‐Book', style: TextStyle(fontSize: 14)),
                style: TextButton.styleFrom(
                  padding: const EdgeInsets.symmetric(
                    horizontal: 16,
                    vertical: 8,
                  ),
                  shape: RoundedRectangleBorder(
                    borderRadius: BorderRadius.circular(8),
                    side: BorderSide(color: theme.colorScheme.primary),
                  ),
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }
}
