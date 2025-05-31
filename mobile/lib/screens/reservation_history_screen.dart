import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:mobile/providers/reservation_provider.dart';
import 'package:mobile/providers/auth_provider.dart';
import 'package:mobile/providers/spot_provider.dart';
import 'package:mobile/widgets/reservation_history_card.dart';
import 'package:mobile/models/reservation.dart';
import 'package:mobile/models/spot.dart';
import 'package:mobile/screens/make_reservation_screen.dart';

class ReservationHistoryScreen extends StatelessWidget {
  const ReservationHistoryScreen({Key? key}) : super(key: key);

  @override
  Widget build(BuildContext context) {
    final authProv = Provider.of<AuthProvider>(context, listen: false);
    final resProv = Provider.of<ReservationProvider>(context);
    final spotProv = Provider.of<SpotProvider>(context, listen: false);

    final currentUser = authProv.currentUser;
    final rawHistory = resProv.historyReservations ?? [];
    final userHistory =
        rawHistory.where((r) => r.userId == currentUser?.id).toList();

    return Scaffold(
      appBar: AppBar(
        title: const Text('My Reservation History'),
        backgroundColor: Colors.white,
        foregroundColor: Theme.of(context).colorScheme.primary,
        elevation: 1,
      ),
      body:
          resProv.isLoadingHistory
              ? const Center(child: CircularProgressIndicator())
              : userHistory.isEmpty
              ? Center(
                child: Text(
                  'No past reservations',
                  style: Theme.of(context).textTheme.bodyLarge,
                ),
              )
              : ListView.separated(
                padding: const EdgeInsets.symmetric(vertical: 16),
                itemCount: userHistory.length,
                separatorBuilder: (_, __) => const SizedBox(height: 12),
                itemBuilder: (ctx, i) {
                  final r = userHistory[i];
                  // Lookup the Spot object for display
                  Spot? spot;
                  try {
                    spot = spotProv.spots!.firstWhere((s) => s.id == r.spotId);
                  } catch (_) {
                    spot = null;
                  }

                  return ReservationHistoryCard(
                    reservation: r,
                    spot: spot,
                    onRebook: () {
                      Navigator.of(context).push(
                        MaterialPageRoute(
                          builder:
                              (_) => MakeReservationScreen(prefilledSpot: spot),
                        ),
                      );
                    },
                  );
                },
              ),
    );
  }
}
