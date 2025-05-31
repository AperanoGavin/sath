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

    return Scaffold(
      appBar: AppBar(title: const Text('Log In')),
      body:
          auth.isLoading
              ? const Center(child: CircularProgressIndicator())
              : Padding(
                padding: const EdgeInsets.all(16.0),
                child: Column(
                  children: [
                    const Text(
                      'Select your user to log in:',
                      style: TextStyle(fontSize: 18),
                    ),
                    const SizedBox(height: 16),
                    Expanded(
                      child: ListView.builder(
                        itemCount: auth.allUsers?.length ?? 0,
                        itemBuilder: (ctx, i) {
                          final user = auth.allUsers![i];
                          return RadioListTile<User>(
                            title: Text(user.name),
                            subtitle: Text('${user.email} (${user.role.key})'),
                            value: user,
                            groupValue: selectedUser,
                            onChanged: (u) {
                              setState(() {
                                selectedUser = u;
                              });
                            },
                          );
                        },
                      ),
                    ),
                    ElevatedButton(
                      onPressed:
                          selectedUser == null
                              ? null
                              : () async {
                                await auth.login(selectedUser!);
                                // Once logged in, RootNavigator will rebuild to show HomeScreen
                              },
                      child: const Text('Log In'),
                    ),
                  ],
                ),
              ),
    );
  }
}
