import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:mobile/providers/reservation_provider.dart';
import 'package:mobile/widgets/reservation_tile.dart';

class CurrentReservationsScreen extends StatelessWidget {
  const CurrentReservationsScreen({Key? key}) : super(key: key);

  @override
  Widget build(BuildContext context) {
    final resProv = Provider.of<ReservationProvider>(context);
    final current = resProv.currentReservations;

    return Scaffold(
      appBar: AppBar(title: const Text('My Current Reservations')),
      body:
          resProv.isLoadingCurrent
              ? const Center(child: CircularProgressIndicator())
              : current == null || current.isEmpty
              ? const Center(child: Text('No current reservations'))
              : ListView.builder(
                itemCount: current.length,
                itemBuilder: (ctx, i) {
                  final r = current[i];
                  return ReservationTile(
                    reservation: r,
                    onCancel: () => resProv.cancelReservation(r.id),
                    onCheckIn: () => resProv.checkInReservation(r.id),
                  );
                },
              ),
    );
  }
}
