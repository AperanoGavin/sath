import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:mobile/providers/auth_provider.dart';
import 'package:mobile/providers/spot_provider.dart';
import 'package:mobile/providers/reservation_provider.dart';
import 'package:mobile/screens/login_screen.dart';
import 'package:mobile/screens/home_screen.dart';

void main() {
  runApp(const ParkingReservationApp());
}

class ParkingReservationApp extends StatelessWidget {
  const ParkingReservationApp({Key? key}) : super(key: key);

  @override
  Widget build(BuildContext context) {
    return MultiProvider(
      providers: [
        ChangeNotifierProvider(create: (_) => AuthProvider()),
        ChangeNotifierProvider(create: (_) => SpotProvider()),
        ChangeNotifierProvider(create: (_) => ReservationProvider()),
      ],
      child: MaterialApp(
        title: 'Parking Reservation',
        theme: ThemeData(primarySwatch: Colors.blue),
        home: const RootNavigator(),
      ),
    );
  }
}

/// RootNavigator checks if the user is logged in; if not, shows LoginScreen.
/// Otherwise, shows HomeScreen.
class RootNavigator extends StatelessWidget {
  const RootNavigator({Key? key}) : super(key: key);

  @override
  Widget build(BuildContext context) {
    final auth = Provider.of<AuthProvider>(context);
    // While AuthProvider is loading from SharedPreferences, show a splash
    if (auth.isLoading) {
      return const Scaffold(body: Center(child: CircularProgressIndicator()));
    }
    if (!auth.isLoggedIn) {
      return const LoginScreen();
    }
    return const HomeScreen();
  }
}
