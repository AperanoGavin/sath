import 'dart:convert';

import 'package:http/http.dart' as http;
import 'package:mobile/constants.dart';
import 'package:mobile/models/spot.dart';
import 'package:mobile/models/reservation.dart';
import 'package:mobile/models/user.dart';

class ApiService {
  final String baseUrl;

  ApiService({this.baseUrl = kApiBaseUrl});

  Map<String, String> _defaultHeaders() => {'Content-Type': 'application/json'};

  Future<List<Spot>> fetchAllSpots() async {
    final url = Uri.parse('$baseUrl/spots');
    final response = await http.get(url, headers: _defaultHeaders());
    if (response.statusCode == 200) {
      final data = jsonDecode(response.body) as Map<String, dynamic>;
      final spotsJson = data['data'] as List;
      return spotsJson.map((e) => Spot.fromJson(e)).toList();
    } else {
      throw Exception('Failed to load spots (status ${response.statusCode})');
    }
  }

  Future<List<String>> fetchSpotCapabilities() async {
    final url = Uri.parse('$baseUrl/spots/capabilities');
    final response = await http.get(url, headers: _defaultHeaders());
    if (response.statusCode == 200) {
      final data = jsonDecode(response.body) as Map<String, dynamic>;
      final caps = (data['data'] as List).cast<String>();
      return caps;
    } else {
      throw Exception(
        'Failed to load spot capabilities (status ${response.statusCode})',
      );
    }
  }

  Future<Spot> fetchSpotById(String spotId) async {
    final url = Uri.parse('$baseUrl/spots/$spotId');
    final response = await http.get(url, headers: _defaultHeaders());
    if (response.statusCode == 200) {
      final data = jsonDecode(response.body) as Map<String, dynamic>;
      return Spot.fromJson(data['data']);
    } else {
      throw Exception('Spot not found (status ${response.statusCode})');
    }
  }

  Future<List<User>> fetchAllUsers() async {
    final url = Uri.parse('$baseUrl/users');
    final response = await http.get(url, headers: _defaultHeaders());
    if (response.statusCode == 200) {
      final data = jsonDecode(response.body) as Map<String, dynamic>;
      final usersJson = data['data'] as List;
      return usersJson.map((e) => User.fromJson(e)).toList();
    } else {
      throw Exception('Failed to load users (status ${response.statusCode})');
    }
  }

  Future<User> fetchUserById(String userId) async {
    final url = Uri.parse('$baseUrl/users/$userId');
    final response = await http.get(url, headers: _defaultHeaders());
    if (response.statusCode == 200) {
      final data = jsonDecode(response.body) as Map<String, dynamic>;
      return User.fromJson(data['data']);
    } else {
      throw Exception('User not found (status ${response.statusCode})');
    }
  }

  Future<List<Reservation>> fetchAllReservations() async {
    final url = Uri.parse('$baseUrl/reservations');
    final response = await http.get(url, headers: _defaultHeaders());
    if (response.statusCode == 200) {
      final data = jsonDecode(response.body) as Map<String, dynamic>;
      final resJson = data['data'] as List;
      return resJson.map((e) => Reservation.fromJson(e)).toList();
    } else {
      throw Exception(
        'Failed to load reservations (status ${response.statusCode})',
      );
    }
  }

  Future<List<Reservation>> fetchReservationHistory() async {
    final url = Uri.parse('$baseUrl/reservations/history');
    final response = await http.get(url, headers: _defaultHeaders());
    if (response.statusCode == 200) {
      final data = jsonDecode(response.body) as Map<String, dynamic>;
      final resJson = data['data'] as List;
      return resJson.map((e) => Reservation.fromJson(e)).toList();
    } else {
      throw Exception(
        'Failed to load reservation history (status ${response.statusCode})',
      );
    }
  }

  Future<Reservation> createReservation({
    required String spotId,
    required String userId,
    required DateTime from,
    required DateTime to,
    bool needsCharger = false,
  }) async {
    final url = Uri.parse('$baseUrl/reservations');
    final body = jsonEncode({
      'spotId': spotId,
      'userId': userId,
      'from': from.toIso8601String(),
      'to': to.toIso8601String(),
      'needsCharger': needsCharger,
    });
    final response = await http.post(
      url,
      headers: _defaultHeaders(),
      body: body,
    );
    if (response.statusCode == 201) {
      final data = jsonDecode(response.body) as Map<String, dynamic>;
      return Reservation.fromJson(data['data']);
    } else {
      final err = jsonDecode(response.body);
      throw Exception(
        'Failed to create reservation: ${err['title'] ?? err['detail']}',
      );
    }
  }

  Future<void> cancelReservation(String reservationId) async {
    final url = Uri.parse('$baseUrl/reservations/$reservationId/cancel');
    final response = await http.put(url, headers: _defaultHeaders());
    if (response.statusCode == 204) {
      return;
    } else {
      final err = jsonDecode(response.body);
      throw Exception(
        'Failed to cancel reservation: ${err['title'] ?? err['detail']}',
      );
    }
  }

  Future<void> checkInReservation(String reservationId) async {
    final url = Uri.parse('$baseUrl/reservations/$reservationId/check-in');
    final response = await http.post(url, headers: _defaultHeaders());
    if (response.statusCode == 204) {
      return;
    } else {
      final err = jsonDecode(response.body);
      throw Exception('Failed to check‐in: ${err['title'] ?? err['detail']}');
    }
  }

  Future<List<Reservation>> fetchSpotCalendar(String spotId) async {
    final url = Uri.parse('$baseUrl/spots/$spotId/calendar');
    final response = await http.get(url, headers: _defaultHeaders());
    if (response.statusCode == 200) {
      final data = jsonDecode(response.body) as Map<String, dynamic>;
      final resJson = data['data'] as List;
      return resJson.map((e) => Reservation.fromJson(e)).toList();
    } else {
      throw Exception(
        'Failed to load spot calendar (status ${response.statusCode})',
      );
    }
  }
}
