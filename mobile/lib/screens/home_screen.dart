import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:mobile/providers/auth_provider.dart';
import 'package:mobile/screens/spots_list_screen.dart';
import 'package:mobile/screens/current_reservations_screen.dart';
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
            tooltip: 'Log Out',
            onPressed: () async {
              await auth.logout();
            },
          ),
        ],
      ),
      body: SingleChildScrollView(
        padding: const EdgeInsets.symmetric(vertical: 24, horizontal: 16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            // 1) View/Book Spots
            ElevatedButton.icon(
              icon: const Icon(Icons.local_parking, size: 28),
              label: const Padding(
                padding: EdgeInsets.symmetric(vertical: 14),
                child: Text(
                  'View / Book Parking Spots',
                  style: TextStyle(fontSize: 16),
                ),
              ),
              onPressed: () {
                Navigator.of(context).push(
                  MaterialPageRoute(builder: (_) => const SpotsListScreen()),
                );
              },
              style: ElevatedButton.styleFrom(
                shape: RoundedRectangleBorder(
                  borderRadius: BorderRadius.circular(8),
                ),
              ),
            ),

            const SizedBox(height: 16),

            // 2) My Current Reservations
            ElevatedButton.icon(
              icon: const Icon(Icons.today_sharp, size: 28),
              label: const Padding(
                padding: EdgeInsets.symmetric(vertical: 14),
                child: Text(
                  'My Current & Future Reservations',
                  style: TextStyle(fontSize: 16),
                ),
              ),
              onPressed: () {
                Navigator.of(context).push(
                  MaterialPageRoute(
                    builder: (_) => const CurrentReservationsScreen(),
                  ),
                );
              },
              style: ElevatedButton.styleFrom(
                shape: RoundedRectangleBorder(
                  borderRadius: BorderRadius.circular(8),
                ),
              ),
            ),

            const SizedBox(height: 16),

            // 3) Reservation History
            ElevatedButton.icon(
              icon: const Icon(Icons.history_toggle_off, size: 28),
              label: const Padding(
                padding: EdgeInsets.symmetric(vertical: 14),
                child: Text(
                  'Reservation History',
                  style: TextStyle(fontSize: 16),
                ),
              ),
              onPressed: () {
                Navigator.of(context).push(
                  MaterialPageRoute(
                    builder: (_) => const ReservationHistoryScreen(),
                  ),
                );
              },
              style: ElevatedButton.styleFrom(
                shape: RoundedRectangleBorder(
                  borderRadius: BorderRadius.circular(8),
                ),
              ),
            ),

            const SizedBox(height: 16),

            // 4) Scan QR to Check‐In
            ElevatedButton.icon(
              icon: const Icon(Icons.qr_code_scanner, size: 28),
              label: const Padding(
                padding: EdgeInsets.symmetric(vertical: 14),
                child: Text(
                  'Scan QR to Check‐In',
                  style: TextStyle(fontSize: 16),
                ),
              ),
              onPressed: () {
                Navigator.of(
                  context,
                ).push(MaterialPageRoute(builder: (_) => const ScanQRScreen()));
              },
              style: ElevatedButton.styleFrom(
                shape: RoundedRectangleBorder(
                  borderRadius: BorderRadius.circular(8),
                ),
              ),
            ),

            const SizedBox(height: 16),

            // 5) Admin Dashboard (if Manager/Secretary)
            if (user.role.key == 'Manager' || user.role.key == 'Secretary')
              ElevatedButton.icon(
                icon: const Icon(Icons.dashboard, size: 28),
                label: const Padding(
                  padding: EdgeInsets.symmetric(vertical: 14),
                  child: Text(
                    'Admin Dashboard',
                    style: TextStyle(fontSize: 16),
                  ),
                ),
                onPressed: () {
                  Navigator.of(context).push(
                    MaterialPageRoute(
                      builder: (_) => const AdminDashboardScreen(),
                    ),
                  );
                },
                style: ElevatedButton.styleFrom(
                  foregroundColor: Colors.deepPurple,
                  shape: RoundedRectangleBorder(
                    borderRadius: BorderRadius.circular(8),
                  ),
                ),
              ),
          ],
        ),
      ),
    );
  }
}
