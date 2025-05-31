import 'package:flutter/material.dart';
import 'package:mobile/models/spot.dart';

class SpotTile extends StatelessWidget {
  final Spot spot;
  final VoidCallback? onTap;

  const SpotTile({Key? key, required this.spot, this.onTap}) : super(key: key);

  @override
  Widget build(BuildContext context) {
    final hasCharger = spot.capabilities.contains('ElectricCharger');
    return Card(
      margin: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
      child: ListTile(
        onTap: onTap,
        title: Text('Spot ${spot.key}', style: const TextStyle(fontSize: 18)),
        subtitle: Text(
          hasCharger ? '⚡ Electric Charger' : 'No charger',
          style: TextStyle(color: hasCharger ? Colors.green : Colors.grey[600]),
        ),
        trailing: const Icon(Icons.arrow_forward_ios, size: 16),
      ),
    );
  }
}
