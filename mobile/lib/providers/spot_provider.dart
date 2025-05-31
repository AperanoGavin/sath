import 'package:flutter/material.dart';
import 'package:mobile/models/spot.dart';
import 'package:mobile/services/api_service.dart';

class SpotProvider extends ChangeNotifier {
  final ApiService _api = ApiService();

  List<Spot>? _spots;
  List<String>? _capabilities;
  bool _loading = false;

  List<Spot>? get spots => _spots;
  List<String>? get capabilities => _capabilities;
  bool get isLoading => _loading;

  SpotProvider() {
    fetchSpots();
    fetchCapabilities();
  }

  Future<void> fetchSpots() async {
    _loading = true;
    notifyListeners();
    try {
      _spots = await _api.fetchAllSpots();
    } catch (e) {
      _spots = [];
    } finally {
      _loading = false;
      notifyListeners();
    }
  }

  Future<void> fetchCapabilities() async {
    try {
      _capabilities = await _api.fetchSpotCapabilities();
    } catch (e) {
      _capabilities = [];
    }
    notifyListeners();
  }
}
