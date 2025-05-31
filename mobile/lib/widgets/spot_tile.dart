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
      elevation: 2,
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
      child: InkWell(
        borderRadius: BorderRadius.circular(12),
        onTap: onTap,
        child: Padding(
          padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 16),
          child: Row(
            children: [
              Container(
                width: 48,
                height: 48,
                decoration: BoxDecoration(
                  color:
                      hasCharger ? Colors.green.shade100 : Colors.grey.shade200,
                  shape: BoxShape.circle,
                ),
                child: Center(
                  child: Icon(
                    Icons.local_parking,
                    size: 28,
                    color:
                        hasCharger
                            ? Colors.green.shade700
                            : Colors.grey.shade700,
                  ),
                ),
              ),
              const SizedBox(width: 16),
              Expanded(
                child: Text(
                  'Spot ${spot.key}',
                  style: Theme.of(
                    context,
                  ).textTheme.bodyLarge!.copyWith(fontWeight: FontWeight.w600),
                ),
              ),
              if (hasCharger) ...[
                Icon(Icons.ev_station, color: Colors.green.shade700, size: 20),
                const SizedBox(width: 6),
                Text('EV', style: TextStyle(color: Colors.green.shade700)),
              ] else ...[
                Icon(Icons.power_off, color: Colors.grey, size: 20),
                const SizedBox(width: 6),
                Text('No', style: TextStyle(color: Colors.grey.shade600)),
              ],
              const SizedBox(width: 8),
              Icon(Icons.chevron_right, color: Colors.grey.shade600),
            ],
          ),
        ),
      ),
    );
  }
}
