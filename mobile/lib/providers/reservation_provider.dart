import 'package:flutter/material.dart';
import 'package:mobile/models/reservation.dart';
import 'package:mobile/services/api_service.dart';

class ReservationProvider extends ChangeNotifier {
  final ApiService _api = ApiService();

  List<Reservation>? _currentReservations;
  List<Reservation>? _historyReservations;
  bool _loadingCurrent = false;
  bool _loadingHistory = false;

  List<Reservation>? get currentReservations => _currentReservations;
  List<Reservation>? get historyReservations => _historyReservations;
  bool get isLoadingCurrent => _loadingCurrent;
  bool get isLoadingHistory => _loadingHistory;

  ReservationProvider() {
    fetchCurrentReservations();
    fetchHistoryReservations();
  }

  Future<void> fetchCurrentReservations() async {
    _loadingCurrent = true;
    notifyListeners();
    try {
      _currentReservations = await _api.fetchAllReservations();
    } catch (e) {
      _currentReservations = [];
    } finally {
      _loadingCurrent = false;
      notifyListeners();
    }
  }

  Future<void> fetchHistoryReservations() async {
    _loadingHistory = true;
    notifyListeners();
    try {
      _historyReservations = await _api.fetchReservationHistory();
    } catch (e) {
      _historyReservations = [];
    } finally {
      _loadingHistory = false;
      notifyListeners();
    }
  }

  Future<Reservation> createReservation({
    required String spotId,
    required String userId,
    required DateTime from,
    required DateTime to,
    bool needsCharger = false,
  }) async {
    final res = await _api.createReservation(
      spotId: spotId,
      userId: userId,
      from: from,
      to: to,
      needsCharger: needsCharger,
    );

    await fetchCurrentReservations();
    return res;
  }

  Future<void> cancelReservation(String reservationId) async {
    await _api.cancelReservation(reservationId);
    await fetchCurrentReservations();
    await fetchHistoryReservations();
  }

  Future<void> checkInReservation(String reservationId) async {
    await _api.checkInReservation(reservationId);
    await fetchCurrentReservations();
  }
}
