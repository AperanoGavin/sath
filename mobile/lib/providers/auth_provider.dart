import 'package:flutter/material.dart';
import 'package:mobile/services/api_service.dart';
import 'package:mobile/models/user.dart';
import 'package:shared_preferences/shared_preferences.dart';
import 'package:mobile/constants.dart';

class AuthProvider extends ChangeNotifier {
  final ApiService _api = ApiService();

  User? _currentUser;
  List<User>? _allUsers;
  bool _loading = false;

  User? get currentUser => _currentUser;
  List<User>? get allUsers => _allUsers;
  bool get isLoading => _loading;
  bool get isLoggedIn => _currentUser != null;

  AuthProvider() {
    _loadFromPrefs();
  }

  Future<void> fetchAllUsers() async {
    _loading = true;
    notifyListeners();
    try {
      _allUsers = await _api.fetchAllUsers();
    } catch (e) {
      _allUsers = [];
    } finally {
      _loading = false;
      notifyListeners();
    }
  }

  Future<void> login(User user) async {
    _currentUser = user;
    final prefs = await SharedPreferences.getInstance();
    await prefs.setString(kUserPrefsKey, user.id);
    await prefs.setString(kUserRoleKey, user.role.key);
    notifyListeners();
  }

  Future<void> logout() async {
    _currentUser = null;
    final prefs = await SharedPreferences.getInstance();
    await prefs.remove(kUserPrefsKey);
    await prefs.remove(kUserRoleKey);
    notifyListeners();
  }

  Future<void> _loadFromPrefs() async {
    final prefs = await SharedPreferences.getInstance();
    final userId = prefs.getString(kUserPrefsKey);
    if (userId != null) {
      try {
        final user = await _api.fetchUserById(userId);
        _currentUser = user;
      } catch (_) {
        _currentUser = null;
      }
    }
    notifyListeners();
  }
}
