import 'package:flutter/material.dart';
import 'package:mobile/providers/reservation_provider.dart';
import 'package:mobile/providers/spot_provider.dart';
import 'package:mobile/widgets/reservation_tile.dart';
import 'package:mobile/widgets/spot_tile.dart';
import 'package:provider/provider.dart';

class AdminDashboardScreen extends StatefulWidget {
  const AdminDashboardScreen({Key? key}) : super(key: key);

  @override
  State<AdminDashboardScreen> createState() => _AdminDashboardScreenState();
}

class _AdminDashboardScreenState extends State<AdminDashboardScreen>
    with SingleTickerProviderStateMixin {
  late TabController _tabController;

  @override
  Widget build(BuildContext context) {
    final spotProv = Provider.of<SpotProvider>(context);
    final resProv = Provider.of<ReservationProvider>(context);

    final spots = spotProv.spots ?? [];
    final currentRes = resProv.currentReservations ?? [];
    final historyRes = resProv.historyReservations ?? [];

    // Simple stats:
    final totalSpots = spots.length;
    final reservedCount = currentRes.length;
    final usedCount = historyRes.length; // simplistic

    final noShowCount =
        currentRes.where((r) => r.status == 'Expired').length; // approximate
    final noShowRate =
        reservedCount > 0 ? noShowCount / reservedCount * 100 : 0.0;

    final electricSpotsUsed =
        currentRes.where((r) {
          final spot = spots.firstWhere((s) => s.id == r.spotId);
          return spot.capabilities.contains('ElectricCharger');
        }).length;
    final electricUsageRate =
        currentRes.isNotEmpty
            ? electricSpotsUsed / currentRes.length * 100
            : 0.0;

    return Scaffold(
      appBar: AppBar(
        title: const Text('Admin Dashboard'),
        bottom: TabBar(
          controller: _tabController,
          tabs: const [
            Tab(text: 'Stats'),
            Tab(text: 'All Spots'),
            Tab(text: 'All Reservations'),
          ],
        ),
      ),
      body: TabBarView(
        controller: _tabController,
        children: [
          // Tab 0: Stats
          Padding(
            padding: const EdgeInsets.all(16),
            child: Column(
              children: [
                Text('Total Spots: $totalSpots'),
                Text('Currently Reserved: $reservedCount'),
                Text('No‐Show Rate: ${noShowRate.toStringAsFixed(1)}%'),
                Text(
                  'Electric Spots Usage: ${electricUsageRate.toStringAsFixed(1)}%',
                ),
                const SizedBox(height: 24),
                ElevatedButton(
                  onPressed: () {
                    // You could implement “create new spot” here
                  },
                  child: const Text('Add New Spot'),
                ),
              ],
            ),
          ),

          // Tab 1: All Spots
          spotProv.isLoading
              ? const Center(child: CircularProgressIndicator())
              : ListView.builder(
                itemCount: spots.length,
                itemBuilder: (ctx, i) {
                  final s = spots[i];
                  return SpotTile(
                    spot: s,
                    onTap: () {
                      // Show detail, calendar, or edit/delete
                    },
                  );
                },
              ),

          // Tab 2: All Reservations
          resProv.isLoadingCurrent
              ? const Center(child: CircularProgressIndicator())
              : ListView.builder(
                itemCount: currentRes.length,
                itemBuilder: (ctx, i) {
                  final r = currentRes[i];
                  return ReservationTile(reservation: r);
                },
              ),
        ],
      ),
    );
  }

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
}
