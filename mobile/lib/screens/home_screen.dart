import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:mobile/providers/auth_provider.dart';
import 'package:mobile/screens/spots_list_screen.dart';
import 'package:mobile/screens/reservation_history_screen.dart';
import 'package:mobile/screens/make_reservation_screen.dart';
import 'package:mobile/screens/scan_qr_screen.dart';
import 'package:mobile/screens/admin_dashboard_screen.dart';
import 'package:mobile/models/user.dart';

class HomeScreen extends StatelessWidget {
  const HomeScreen({Key? key}) : super(key: key);

  @override
  Widget build(BuildContext context) {
    final auth = Provider.of<AuthProvider>(context);
    final user = auth.currentUser!; // guaranteed non-null here
    return Scaffold(
      appBar: AppBar(
        title: Text('Welcome, ${user.name}!'),
        actions: [
          IconButton(
            icon: const Icon(Icons.logout),
            onPressed: () async {
              await auth.logout();
            },
          ),
        ],
      ),
      body: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            ElevatedButton.icon(
              icon: const Icon(Icons.local_parking),
              label: const Text('View/Book Parking Spots'),
              onPressed: () {
                Navigator.of(context).push(
                  MaterialPageRoute(builder: (_) => const SpotsListScreen()),
                );
              },
            ),
            const SizedBox(height: 12),
            ElevatedButton.icon(
              icon: const Icon(Icons.history),
              label: const Text('My Current & Future Reservations'),
              onPressed: () {
                Navigator.of(context).push(
                  MaterialPageRoute(
                    builder: (_) => const MakeReservationScreen(),
                  ),
                );
              },
            ),
            const SizedBox(height: 12),
            ElevatedButton.icon(
              icon: const Icon(Icons.history_toggle_off),
              label: const Text('Reservation History'),
              onPressed: () {
                Navigator.of(context).push(
                  MaterialPageRoute(
                    builder: (_) => const ReservationHistoryScreen(),
                  ),
                );
              },
            ),
            const SizedBox(height: 12),
            ElevatedButton.icon(
              icon: const Icon(Icons.qr_code_scanner),
              label: const Text('Scan QR to Check‐In'),
              onPressed: () {
                Navigator.of(
                  context,
                ).push(MaterialPageRoute(builder: (_) => const ScanQRScreen()));
              },
            ),
            const SizedBox(height: 12),
            // If user is a Manager or Secretary/Admin, show dashboard link
            if (user.role.key == 'Manager' || user.role.key == 'Secretary')
              ElevatedButton.icon(
                icon: const Icon(Icons.dashboard),
                label: const Text('Admin Dashboard'),
                onPressed: () {
                  Navigator.of(context).push(
                    MaterialPageRoute(
                      builder: (_) => const AdminDashboardScreen(),
                    ),
                  );
                },
              ),
          ],
        ),
      ),
    );
  }
}
