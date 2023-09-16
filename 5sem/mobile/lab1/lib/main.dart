import 'package:flutter/material.dart';
import 'package:lab1/numberInput.dart';

void main() {
  runApp(const MyApp());
}

class MyApp extends StatelessWidget {
  const MyApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      debugShowCheckedModeBanner: false,
      home: Scaffold(
        backgroundColor: Colors.cyanAccent,
        appBar: AppBar(
          backgroundColor: Colors.cyan,
          title: const Text('Lab1'),
        ),
        body: const NumberInputWidget(),
      ),
    );
  }
}
