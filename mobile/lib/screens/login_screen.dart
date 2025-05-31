import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:mobile/providers/auth_provider.dart';
import 'package:mobile/models/user.dart';

class LoginScreen extends StatefulWidget {
  const LoginScreen({Key? key}) : super(key: key);

  @override
  State<LoginScreen> createState() => _LoginScreenState();
}

class _LoginScreenState extends State<LoginScreen> {
  User? selectedUser;

  @override
  void initState() {
    super.initState();
    final auth = Provider.of<AuthProvider>(context, listen: false);
    auth.fetchAllUsers();
  }

  @override
  Widget build(BuildContext context) {
    final auth = Provider.of<AuthProvider>(context);
    final theme = Theme.of(context);

    return Scaffold(
      backgroundColor: Colors.white,
      body: Center(
        child:
            auth.isLoading
                ? const CircularProgressIndicator()
                : SingleChildScrollView(
                  padding: const EdgeInsets.symmetric(horizontal: 24),
                  child: Column(
                    children: [
                      // (Optional) replace with your own logo
                      const SizedBox(height: 48),
                      Icon(
                        Icons.local_parking,
                        size: 100,
                        color: theme.colorScheme.primary,
                      ),
                      const SizedBox(height: 24),
                      Text(
                        'Select Your User',
                        style: theme.textTheme.headlineSmall!.copyWith(
                          fontWeight: FontWeight.bold,
                        ),
                      ),
                      const SizedBox(height: 24),

                      // List of users with radio buttons
                      Card(
                        elevation: 2,
                        shape: RoundedRectangleBorder(
                          borderRadius: BorderRadius.circular(12),
                        ),
                        child: Padding(
                          padding: const EdgeInsets.all(16.0),
                          child: Column(
                            children: [
                              for (var user in auth.allUsers ?? []) ...[
                                RadioListTile<User>(
                                  title: Text(user.name),
                                  subtitle: Text(
                                    '${user.email} (${user.role.key})',
                                    style: theme.textTheme.bodySmall!.copyWith(
                                      color: Colors.grey.shade600,
                                    ),
                                  ),
                                  value: user,
                                  groupValue: selectedUser,
                                  onChanged: (u) {
                                    setState(() {
                                      selectedUser = u;
                                    });
                                  },
                                ),
                                const Divider(height: 1),
                              ],
                            ],
                          ),
                        ),
                      ),

                      const SizedBox(height: 24),

                      SizedBox(
                        width: double.infinity,
                        height: 64,
                        child: ElevatedButton.icon(
                          icon: const Icon(Icons.login, color: Colors.white),
                          label: const Text('Log In'),
                          style: ElevatedButton.styleFrom(
                            backgroundColor: theme.colorScheme.primary,
                            shape: RoundedRectangleBorder(
                              borderRadius: BorderRadius.circular(8),
                            ),
                          ),
                          onPressed:
                              selectedUser == null
                                  ? null
                                  : () async {
                                    await auth.login(selectedUser!);
                                  },
                        ),
                      ),
                    ],
                  ),
                ),
      ),
    );
  }
}
