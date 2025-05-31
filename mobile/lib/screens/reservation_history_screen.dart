import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:mobile/providers/reservation_provider.dart';
import 'package:mobile/widgets/reservation_tile.dart';

class ReservationHistoryScreen extends StatelessWidget {
  const ReservationHistoryScreen({Key? key}) : super(key: key);

  @override
  Widget build(BuildContext context) {
    final resProv = Provider.of<ReservationProvider>(context);
    final history = resProv.historyReservations;

    return Scaffold(
      appBar: AppBar(title: const Text('Reservation History')),
      body:
          resProv.isLoadingHistory
              ? const Center(child: CircularProgressIndicator())
              : history == null || history.isEmpty
              ? const Center(child: Text('No past reservations'))
              : ListView.builder(
                itemCount: history.length,
                itemBuilder: (ctx, i) {
                  final r = history[i];
                  return ReservationTile(reservation: r);
                },
              ),
    );
  }
}
