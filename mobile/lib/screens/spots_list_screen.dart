import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:mobile/providers/spot_provider.dart';
import 'package:mobile/models/spot.dart';
import 'package:mobile/widgets/spot_tile.dart';
import 'package:mobile/screens/make_reservation_screen.dart';

class SpotsListScreen extends StatelessWidget {
  const SpotsListScreen({Key? key}) : super(key: key);

  @override
  Widget build(BuildContext context) {
    final spotProv = Provider.of<SpotProvider>(context);
    final spots = spotProv.spots;

    return Scaffold(
      appBar: AppBar(title: const Text('All Parking Spots')),
      body:
          spotProv.isLoading
              ? const Center(child: CircularProgressIndicator())
              : spots == null || spots.isEmpty
              ? const Center(child: Text('No spots available'))
              : ListView.builder(
                itemCount: spots.length,
                itemBuilder: (ctx, i) {
                  final spot = spots[i];
                  return SpotTile(
                    spot: spot,
                    onTap: () {
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
