import 'package:flutter/material.dart';
import 'package:mobile/models/reservation.dart';
import 'package:intl/intl.dart';

class ReservationTile extends StatelessWidget {
  final Reservation reservation;
  final VoidCallback? onCancel;
  final VoidCallback? onCheckIn;

  const ReservationTile({
    Key? key,
    required this.reservation,
    this.onCancel,
    this.onCheckIn,
  }) : super(key: key);

  @override
  Widget build(BuildContext context) {
    final from = DateFormat.yMd().format(reservation.from);
    final to = DateFormat.yMd().format(reservation.to);
    return Card(
      margin: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
      child: ListTile(
        title: Text('Spot ${reservation.spotId}'),
        subtitle: Text('$from → $to\nStatus: ${reservation.status}'),
        isThreeLine: true,
        trailing: Wrap(
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
          ],
        ),
      ),
    );
  }
}
