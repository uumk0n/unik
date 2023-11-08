import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'dart:convert';
import 'dart:io';
import 'package:path_provider/path_provider.dart';

class IrisApp extends StatefulWidget {
  @override
  _IrisAppState createState() => _IrisAppState();
}

class _IrisAppState extends State<IrisApp> {
  String selectedMetric = 'Евклидова';
  int k = 3;
  bool useWeightedVoting = false;
  TextEditingController weightsController = TextEditingController();

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text('Классификация ирисов'),
      ),
      body: Center(
        child: Container(
          width: 300, // Установите ширину контейнера в 300 пикселей
          child: Column(
            mainAxisAlignment: MainAxisAlignment.center,
            children: <Widget>[
              Container(
                width: double
                    .infinity, // Чтобы ширина TextField была на всю ширину контейнера
                child: TextField(
                  keyboardType: TextInputType.number,
                  inputFormatters: <TextInputFormatter>[
                    FilteringTextInputFormatter
                        .digitsOnly, // Ограничиваем ввод только цифрами
                  ],
                  decoration: InputDecoration(
                      labelText: 'Количество ближайших соседей (k)'),
                  onChanged: (value) {
                    setState(() {
                      k = int.tryParse(value) ?? 1;
                    });
                  },
                ),
              ),
              Container(
                width: double
                    .infinity, // Чтобы ширина DropdownButton была на всю ширину контейнера
                child: DropdownButton<String>(
                  value: selectedMetric,
                  onChanged: (newValue) {
                    setState(() {
                      selectedMetric = newValue.toString();
                    });
                  },
                  items: ['Евклидова', 'Хемминга', 'Чебышева', 'Косинусная']
                      .map((String value) {
                    return DropdownMenuItem<String>(
                      value: value,
                      child: Text(value),
                    );
                  }).toList(),
                ),
              ),
              Container(
                width: double.infinity,
                child: TextField(
                  controller: weightsController, // Привязываем контроллер
                  keyboardType: TextInputType.text,
                  decoration: InputDecoration(
                      labelText:
                          'Введите веса для признаков через запятую (например, 1,2,1,1):'),
                ),
              ),
              Container(
                width: double
                    .infinity, // Чтобы ширина Switch была на всю ширину контейнера
                child: SwitchListTile(
                  value: useWeightedVoting,
                  onChanged: (newValue) {
                    setState(() {
                      useWeightedVoting = newValue;
                    });
                  },
                  title: Text('Нормализировать данные'),
                ),
              ),
              Container(
                width: double
                    .infinity, // Чтобы ширина кнопки была на всю ширину контейнера
                child: ElevatedButton(
                  onPressed: () {
                    // Вызов функции для выполнения классификации с параметрами k, selectedMetric и useWeightedVoting
                    classifyIris(k, selectedMetric, useWeightedVoting,
                        weightsController.text, context);
                  },
                  child: Text('Классифицировать'),
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }

  void classifyIris(int k, String metric, bool useWeightedVoting,
      String weightsInput, BuildContext context) {
    // Обработка весов из входных данных
    List<double> weights = weightsInput
        .split(',')
        .map((weight) => double.tryParse(weight) ?? 1.0)
        .toList();

    saveParameters(k, metric, useWeightedVoting, weights);
    runPythonScript(context);
  }

  @override
  void dispose() {
    weightsController.dispose(); // Очистка контроллера при завершении
    super.dispose();
  }
}

void saveParameters(
    int k, String metric, bool useWeightedVoting, List<double> weights) {
  final parameters = {
    'k': k,
    'metric': metric,
    'useWeightedVoting': useWeightedVoting,
    'weights': weights,
  };

  final jsonString = jsonEncode(parameters);

  final file = File('parameters.json');
  file.writeAsStringSync(jsonString);
}

void runPythonScript(BuildContext context) async {
  final pythonScriptPath = 'python_script.py';

  final process = await Process.start('python', [pythonScriptPath]);

  String pythonOutput = ''; // Создайте переменную для хранения вывода

  process.stdout.transform(utf8.decoder).listen((data) {
    print('stdout: $data');
    pythonOutput += data; // Сохраните вывод в переменной
  });

  process.stderr.transform(utf8.decoder).listen((data) {
    print('stderr: $data');
  });

  final exitCode = await process.exitCode;
  print('Питоновский скрипт завершился с кодом: $exitCode');

  showDialog(
    context: context,
    builder: (BuildContext context) {
      return Dialog(
        child: Text(pythonOutput), // Выводите данные в модальном окне
      );
    },
  );
}
