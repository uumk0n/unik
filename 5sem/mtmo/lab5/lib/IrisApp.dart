import 'package:flutter/material.dart';
import 'package:flutter/services.dart';

class IrisApp extends StatefulWidget {
  @override
  _IrisAppState createState() => _IrisAppState();
}

class _IrisAppState extends State<IrisApp> {
  String selectedMetric = 'Эвклидова';
  int k = 3;
  bool useWeightedVoting = false;

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
                  items: ['Эвклидова', 'Хемминга', 'Чебышева', 'Косинусная']
                      .map((String value) {
                    return DropdownMenuItem<String>(
                      value: value,
                      child: Text(value),
                    );
                  }).toList(),
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
                  title: Text('Использовать взвешенное голосование'),
                ),
              ),
              Container(
                width: double
                    .infinity, // Чтобы ширина кнопки была на всю ширину контейнера
                child: ElevatedButton(
                  onPressed: () {
                    // Вызов функции для выполнения классификации с параметрами k, selectedMetric и useWeightedVoting
                    classifyIris(k, selectedMetric, useWeightedVoting);
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

  // Метод для выполнения классификации на основе параметров k, метрики и весов
  void classifyIris(int k, String metric, bool useWeightedVoting) {
    // Здесь вы можете вставить логику вызова Python-скрипта с параметрами k, metric и useWeightedVoting
    // и обработать результат классификации.
  }
}
