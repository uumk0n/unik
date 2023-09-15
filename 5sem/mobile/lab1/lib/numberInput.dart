import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:intl/intl.dart';

class NumberInputWidget extends StatefulWidget {
  const NumberInputWidget({Key? key}) : super(key: key);

  @override
  _NumberInputWidgetState createState() => _NumberInputWidgetState();
}

class _NumberInputWidgetState extends State<NumberInputWidget> {
  TextEditingController _textFieldController1 = TextEditingController();
  TextEditingController _textFieldController2 = TextEditingController();
  String result = '';

  @override
  void dispose() {
    _textFieldController1.dispose();
    _textFieldController2.dispose();
    super.dispose();
  }

  bool _validateInput(String input) {
    final RegExp regex = RegExp(r'^[\d,]+$');
    return regex.hasMatch(input);
  }

  void _calculateSum() {
    String number1 = _textFieldController1.text;
    String number2 = _textFieldController2.text;

    // Используем NumberFormat для правильной обработки десятичных чисел
    NumberFormat format = NumberFormat.decimalPattern('en_US');
    double num1 = format.parse(number1.replaceAll(',', '.')).toDouble();
    double num2 = format.parse(number2.replaceAll(',', '.')).toDouble();

    double sum = num1 + num2;

    setState(() {
      result = 'Сумма: ${format.format(sum)}'; // Форматируем результат
    });
  }

  @override
  Widget build(BuildContext context) {
    return Center(
      child: Column(
        mainAxisAlignment: MainAxisAlignment.center,
        children: <Widget>[
          Container(
            constraints: BoxConstraints(maxWidth: 100.0),
            child: TextField(
              controller: _textFieldController1,
              decoration: InputDecoration(
                labelText: 'Введите число 1',
              ),
              keyboardType: TextInputType.number,
              inputFormatters: <TextInputFormatter>[
                FilteringTextInputFormatter.allow(RegExp(r'^[\d,]+$')),
              ],
            ),
          ),
          Container(
            constraints: BoxConstraints(maxWidth: 100.0),
            child: TextField(
              controller: _textFieldController2,
              decoration: InputDecoration(
                labelText: 'Введите число 2',
              ),
              keyboardType: TextInputType.number,
              inputFormatters: <TextInputFormatter>[
                FilteringTextInputFormatter.allow(RegExp(r'^[\d,]+$')),
              ],
            ),
          ),
          ElevatedButton(
            onPressed: () {
              _calculateSum();
            },
            child: Text('Вычислить'),
          ),
          Text(
            result,
            style: TextStyle(fontSize: 20),
          ),
        ],
      ),
    );
  }
}
