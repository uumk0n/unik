import 'package:flutter/material.dart';
import 'package:lab1/numberInput.dart';

void main() {
  runApp(MyApp());
}

class MyApp extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      home: Scaffold(
        appBar: AppBar(
          title: Text('Lab1'),
        ),
        body: NumberInputWidget(),
      ),
    );
  }
}
