import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:mobile/providers/reservation_provider.dart';
import 'package:mobile/widgets/current_reservation_card.dart';

class CurrentReservationsScreen extends StatelessWidget {
  const CurrentReservationsScreen({Key? key}) : super(key: key);

  @override
  Widget build(BuildContext context) {
    final resProv = Provider.of<ReservationProvider>(context);
    final current = resProv.currentReservations;

    return Scaffold(
      appBar: AppBar(
        title: const Text('My Current Reservations'),
        backgroundColor: Colors.white,
        foregroundColor: Theme.of(context).colorScheme.primary,
        elevation: 1,
      ),
      body:
          resProv.isLoadingCurrent
              ? const Center(child: CircularProgressIndicator())
              : (current == null || current.isEmpty)
              ? Center(
                child: Text(
                  'You have no active reservations.',
                  style: Theme.of(context).textTheme.bodyLarge,
                ),
              )
              : ListView.separated(
                padding: const EdgeInsets.symmetric(vertical: 16),
                itemCount: current.length,
                separatorBuilder: (_, __) => const SizedBox(height: 12),
                itemBuilder: (ctx, i) {
                  final r = current[i];
                  return CurrentReservationCard(
                    reservation: r,
                    onCancel: () => resProv.cancelReservation(r.id),
                    onCheckIn: () => resProv.checkInReservation(r.id),
                  );
                },
              ),
    );
  }
}
