import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:mobile/providers/spot_provider.dart';
import 'package:mobile/providers/reservation_provider.dart';
import 'package:mobile/widgets/spot_tile.dart';
import 'package:mobile/widgets/current_reservation_card.dart';
import 'package:mobile/models/spot.dart';
import 'package:mobile/models/reservation.dart';

class AdminDashboardScreen extends StatefulWidget {
  const AdminDashboardScreen({Key? key}) : super(key: key);

  @override
  State<AdminDashboardScreen> createState() => _AdminDashboardScreenState();
}

class _AdminDashboardScreenState extends State<AdminDashboardScreen>
    with SingleTickerProviderStateMixin {
  late TabController _tabController;

  @override
  void initState() {
    super.initState();
    _tabController = TabController(length: 3, vsync: this);
    final spotProv = Provider.of<SpotProvider>(context, listen: false);
    spotProv.fetchSpots();
    final resProv = Provider.of<ReservationProvider>(context, listen: false);
    resProv.fetchCurrentReservations();
    resProv.fetchHistoryReservations();
  }

  @override
  Widget build(BuildContext context) {
    final spotProv = Provider.of<SpotProvider>(context);
    final resProv = Provider.of<ReservationProvider>(context);

    final List<Spot> spots = spotProv.spots ?? [];
    final List<Reservation> currentRes = resProv.currentReservations ?? [];
    final List<Reservation> historyRes = resProv.historyReservations ?? [];

    final totalSpots = spots.length;
    final reservedCount = currentRes.length;
    final usedCount = historyRes.length;

    final noShowCount =
        currentRes.where((r) => r.status == 'Expired').length; // approx
    final noShowRate =
        reservedCount > 0 ? noShowCount / reservedCount * 100 : 0.0;

    final electricSpotsUsed =
        currentRes.where((r) {
          final sp = spots.firstWhere((s) => s.id == r.spotId);
          return sp.capabilities.contains('ElectricCharger');
        }).length;
    final electricUsageRate =
        currentRes.isNotEmpty
            ? electricSpotsUsed / currentRes.length * 100
            : 0.0;

    return Scaffold(
      appBar: AppBar(
        title: const Text('Admin Dashboard'),
        backgroundColor: Colors.white,
        foregroundColor: Theme.of(context).colorScheme.primary,
        elevation: 1,
        bottom: TabBar(
          controller: _tabController,
          labelColor: Theme.of(context).colorScheme.primary,
          unselectedLabelColor: Colors.grey.shade600,
          indicatorColor: Theme.of(context).colorScheme.primary,
          tabs: const [
            Tab(text: 'Stats'),
            Tab(text: 'Spots'),
            Tab(text: 'Reservations'),
          ],
        ),
      ),
      body: TabBarView(
        controller: _tabController,
        children: [
          // ----- Tab 0: Stats -----
          SingleChildScrollView(
            padding: const EdgeInsets.all(16),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.stretch,
              children: [
                // Total Spots card
                Card(
                  elevation: 2,
                  shape: RoundedRectangleBorder(
                    borderRadius: BorderRadius.circular(12),
                  ),
                  child: Padding(
                    padding: const EdgeInsets.all(16),
                    child: Row(
                      children: [
                        Icon(Icons.local_parking, color: Colors.blue, size: 32),
                        const SizedBox(width: 16),
                        Expanded(
                          child: Text(
                            'Total Spots',
                            style: Theme.of(context).textTheme.titleMedium!
                                .copyWith(fontWeight: FontWeight.bold),
                          ),
                        ),
                        Text(
                          '$totalSpots',
                          style: Theme.of(context).textTheme.headlineSmall!
                              .copyWith(color: Colors.blue),
                        ),
                      ],
                    ),
                  ),
                ),

                const SizedBox(height: 12),

                // Currently Reserved
                Card(
                  elevation: 2,
                  shape: RoundedRectangleBorder(
                    borderRadius: BorderRadius.circular(12),
                  ),
                  child: Padding(
                    padding: const EdgeInsets.all(16),
                    child: Row(
                      children: [
                        Icon(
                          Icons.event_available,
                          color: Colors.orange,
                          size: 32,
                        ),
                        const SizedBox(width: 16),
                        Expanded(
                          child: Text(
                            'Currently Reserved',
                            style: Theme.of(context).textTheme.titleMedium!
                                .copyWith(fontWeight: FontWeight.bold),
                          ),
                        ),
                        Text(
                          '$reservedCount',
                          style: Theme.of(context).textTheme.headlineSmall!
                              .copyWith(color: Colors.orange),
                        ),
                      ],
                    ),
                  ),
                ),

                const SizedBox(height: 12),

                // No‐Show Rate
                Card(
                  elevation: 2,
                  shape: RoundedRectangleBorder(
                    borderRadius: BorderRadius.circular(12),
                  ),
                  child: Padding(
                    padding: const EdgeInsets.all(16),
                    child: Row(
                      children: [
                        Icon(
                          Icons.event_busy,
                          color: Colors.red.shade700,
                          size: 32,
                        ),
                        const SizedBox(width: 16),
                        Expanded(
                          child: Text(
                            'No‐Show Rate',
                            style: Theme.of(context).textTheme.titleMedium!
                                .copyWith(fontWeight: FontWeight.bold),
                          ),
                        ),
                        Text(
                          '${noShowRate.toStringAsFixed(1)}%',
                          style: Theme.of(context).textTheme.headlineSmall!
                              .copyWith(color: Colors.red.shade700),
                        ),
                      ],
                    ),
                  ),
                ),

                const SizedBox(height: 12),

                // Electric Usage
                Card(
                  elevation: 2,
                  shape: RoundedRectangleBorder(
                    borderRadius: BorderRadius.circular(12),
                  ),
                  child: Padding(
                    padding: const EdgeInsets.all(16),
                    child: Row(
                      children: [
                        Icon(
                          Icons.ev_station,
                          color: Colors.green.shade700,
                          size: 32,
                        ),
                        const SizedBox(width: 16),
                        Expanded(
                          child: Text(
                            'Electric Spots Usage',
                            style: Theme.of(context).textTheme.titleMedium!
                                .copyWith(fontWeight: FontWeight.bold),
                          ),
                        ),
                        Text(
                          '${electricUsageRate.toStringAsFixed(1)}%',
                          style: Theme.of(context).textTheme.headlineSmall!
                              .copyWith(color: Colors.green.shade700),
                        ),
                      ],
                    ),
                  ),
                ),

                const SizedBox(height: 24),

                ElevatedButton.icon(
                  onPressed: () {
                    // Add new spot flow
                  },
                  icon: const Icon(Icons.add, color: Colors.white),
                  label: const Padding(
                    padding: EdgeInsets.symmetric(vertical: 14),
                    child: Text(
                      'Add New Spot',
                      style: TextStyle(fontSize: 16, color: Colors.white),
                    ),
                  ),
                  style: ElevatedButton.styleFrom(
                    backgroundColor: Theme.of(context).colorScheme.primary,
                    shape: RoundedRectangleBorder(
                      borderRadius: BorderRadius.circular(8),
                    ),
                  ),
                ),
              ],
            ),
          ),

          // ----- Tab 1: All Spots -----
          spotProv.isLoading
              ? const Center(child: CircularProgressIndicator())
              : spots.isEmpty
              ? Center(
                child: Text(
                  'No spots available',
                  style: Theme.of(context).textTheme.bodyLarge,
                ),
              )
              : ListView.builder(
                padding: const EdgeInsets.symmetric(vertical: 16),
                itemCount: spots.length,
                itemBuilder: (ctx, i) {
                  final s = spots[i];
                  return Padding(
                    padding: const EdgeInsets.symmetric(vertical: 6),
                    child: SpotTile(
                      spot: s,
                      onTap: () {
                        // spot detail / calendar / edit
                      },
                    ),
                  );
                },
              ),

          // ----- Tab 2: All Reservations -----
          resProv.isLoadingCurrent
              ? const Center(child: CircularProgressIndicator())
              : currentRes.isEmpty
              ? Center(
                child: Text(
                  'No active reservations',
                  style: Theme.of(context).textTheme.bodyLarge,
                ),
              )
              : ListView.separated(
                padding: const EdgeInsets.symmetric(vertical: 16),
                itemCount: currentRes.length,
                separatorBuilder: (_, __) => const SizedBox(height: 12),
                itemBuilder: (ctx, i) {
                  final r = currentRes[i];
                  return CurrentReservationCard(
                    reservation: r,
                    onCancel: () => resProv.cancelReservation(r.id),
                    onCheckIn: () => resProv.checkInReservation(r.id),
                  );
                },
              ),
        ],
      ),
    );
  }
}
