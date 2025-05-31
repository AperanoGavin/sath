import 'dart:io';

import 'package:flutter/material.dart';
import 'package:qr_code_scanner/qr_code_scanner.dart';
import 'package:provider/provider.dart';
import 'package:mobile/providers/reservation_provider.dart';

class ScanQRScreen extends StatefulWidget {
  const ScanQRScreen({Key? key}) : super(key: key);

  @override
  State<ScanQRScreen> createState() => _ScanQRScreenState();
}

class _ScanQRScreenState extends State<ScanQRScreen> {
  final GlobalKey qrKey = GlobalKey(debugLabel: 'QR');
  QRViewController? controller;
  bool _scanned = false;

  @override
  void reassemble() {
    super.reassemble();
    if (Platform.isAndroid) {
      controller!.pauseCamera();
    }
    controller!.resumeCamera();
  }

  @override
  void dispose() {
    controller?.dispose();
    super.dispose();
  }

  void _onQRViewCreated(QRViewController ctrl) {
    controller = ctrl;
    ctrl.scannedDataStream.listen((scanData) async {
      if (_scanned) return;
      _scanned = true;
      final scannedText = scanData.code;
      // We expect scannedText to be the reservation ID (string GUID)
      if (scannedText != null) {
        try {
          final reservationProv = Provider.of<ReservationProvider>(
            context,
            listen: false,
          );
          await reservationProv.checkInReservation(scannedText);
          // Show a success dialog
          await showDialog(
            context: context,
            builder:
                (_) => AlertDialog(
                  title: const Text('Checked‐In'),
                  content: Text(
                    'Reservation ID $scannedText has been checked in.',
                  ),
                  actions: [
                    TextButton(
                      onPressed: () {
                        Navigator.of(context).pop();
                        Navigator.of(context).pop();
                      },
                      child: const Text('OK'),
                    ),
                  ],
                ),
          );
        } catch (e) {
          await showDialog(
            context: context,
            builder:
                (_) => AlertDialog(
                  title: const Text('Error'),
                  content: Text(e.toString()),
                  actions: [
                    TextButton(
                      onPressed: () {
                        Navigator.of(context).pop();
                        Navigator.of(context).pop();
                      },
                      child: const Text('OK'),
                    ),
                  ],
                ),
          );
        }
      }
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Scan QR to Check‐In')),
      body: Column(
        children: [
          Expanded(
            flex: 4,
            child: QRView(key: qrKey, onQRViewCreated: _onQRViewCreated),
          ),
          const Expanded(
            flex: 1,
            child: Center(child: Text('Point camera at reservation QR code')),
          ),
        ],
      ),
    );
  }
}
