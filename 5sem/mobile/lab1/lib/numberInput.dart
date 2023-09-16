import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:intl/intl.dart';

class NumberInputWidget extends StatefulWidget {
  const NumberInputWidget({Key? key}) : super(key: key);

  @override
  // ignore: library_private_types_in_public_api
  _NumberInputWidgetState createState() => _NumberInputWidgetState();
}

class _NumberInputWidgetState extends State<NumberInputWidget> {
  final TextEditingController _textFieldController1 = TextEditingController();
  final TextEditingController _textFieldController2 = TextEditingController();
  String result = '';

  @override
  void dispose() {
    _textFieldController1.dispose();
    _textFieldController2.dispose();
    super.dispose();
  }

  void _calculateSum() {
    String number1 = _textFieldController1.text;
    String number2 = _textFieldController2.text;

    if (number1.isEmpty || number2.isEmpty) {
      return;
    }
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
            constraints: const BoxConstraints(maxWidth: 100.0),
            child: TextField(
              controller: _textFieldController1,
              decoration: const InputDecoration(
                labelText: 'Введите число 1',
              ),
              keyboardType: TextInputType.number,
              inputFormatters: <TextInputFormatter>[
                FilteringTextInputFormatter.allow(RegExp(r'^[\d,]+$')),
              ],
            ),
          ),
          Container(
            constraints: const BoxConstraints(maxWidth: 100.0),
            child: TextField(
              controller: _textFieldController2,
              decoration: const InputDecoration(
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
            child: const Text('Вычислить'),
          ),
          Text(
            result,
            style: const TextStyle(fontSize: 20),
          ),
        ],
      ),
    );
  }
}
