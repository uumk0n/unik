import 'package:flutter/material.dart';
import 'package:fluttertoast/fluttertoast.dart';
import 'package:lab3/Question.dart';
import 'package:http/http.dart' as http;
import 'dart:convert';
import 'package:translator/translator.dart';

class GeoQuizApp extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return MaterialApp(
        home: Scaffold(
          appBar: AppBar(
            title: Text('GeoQuiz'),
            backgroundColor: Colors.amber,
          ),
          body: QuizScreen(),
          backgroundColor: Colors.amberAccent,
        ),
        debugShowCheckedModeBanner: false);
  }
}

class QuizScreen extends StatefulWidget {
  @override
  _QuizScreenState createState() => _QuizScreenState();
}

Future<List<Question>> fetchQuestions() async {
  final response =
      await http.get(Uri.parse('https://quotable.io/quotes?page=1'));

  if (response.statusCode == 200) {
    final jsonResponse = json.decode(response.body);
    final results = jsonResponse['results'] as List;

    final questions = <Question>[];

    for (var result in results) {
      final content = result['content'];
      final translatedContent = await translateToRussian(content);
      questions.add(Question(translatedContent));
    }

    return questions;
  } else {
    throw Exception('Failed to load questions');
  }
}

Future<String> translateToRussian(String textToTranslate) async {
  final translator = GoogleTranslator();

  final translation = await translator.translate(
    textToTranslate,
    from: 'en',
    to: 'ru',
  );

  return translation.text;
}

class _QuizScreenState extends State<QuizScreen> {
  Map<int, bool> _userAnswers = {};
  List<Question> questions = [];

  int currentQuestionIndex = 0;

  @override
  void initState() {
    super.initState();
    initQuestions();
  }

  void initQuestions() {
    fetchQuestions().then((fetchedQuestions) {
      setState(() {
        questions = fetchedQuestions;
      });
    }).catchError((error) {
      // Обработка ошибки
    });
  }

  void checkAnswer(bool userAnswer) {
    String message;

    if (userAnswer) {
      message = 'Вы ответили "Правда"';
    } else {
      message = 'Вы ответили "Неправда"';
    }

    // Вывод Toast сообщения
    Fluttertoast.showToast(
      msg: message,
      toastLength: Toast.LENGTH_SHORT,
      gravity: ToastGravity.BOTTOM,
      timeInSecForIosWeb: 1,
      backgroundColor: Colors.blue,
      textColor: Colors.white,
      fontSize: 16.0,
    );

    _userAnswers[currentQuestionIndex] = userAnswer;

    if (currentQuestionIndex < questions.length - 1) {
      setState(() {
        currentQuestionIndex++;
      });
    } else {
      int correctAnswersCount =
          _userAnswers.values.where((answer) => answer).length;
      double score = (correctAnswersCount / questions.length) * 100;

      showDialog(
        context: context,
        builder: (context) {
          return AlertDialog(
            title: Text("Статистика"),
            content: Text("Вы согласились: $correctAnswersCount раз\n"
                "Это ${score.toStringAsFixed(2)}%  от всех вопросов"),
            actions: <Widget>[
              TextButton(
                onPressed: () {
                  Navigator.of(context).pop();
                  _resetQuiz();
                },
                child: Text("OK"),
              ),
            ],
          );
        },
      );
    }
  }

  void _resetQuiz() {
    setState(() {
      currentQuestionIndex = 0;
      _userAnswers.clear(); // Очистить ответы пользователя
    });
  }

  @override
  Widget build(BuildContext context) {
    return Center(
      child: Column(
        mainAxisAlignment: MainAxisAlignment.center,
        children: <Widget>[
          questionText(),
          SizedBox(height: 20.0),
          Row(
            // Размещаем кнопки в одну горизонтальную линию
            mainAxisAlignment: MainAxisAlignment.center,
            children: <Widget>[
              TrueBtn(),
              FalseBtn(),
            ],
          ),
        ],
      ),
    );
  }

  ElevatedButton FalseBtn() {
    return ElevatedButton(
        onPressed: () {
          // Обработка нажатия кнопки "False"
          checkAnswer(false);
        },
        style: ElevatedButton.styleFrom(
          primary: Colors.cyanAccent, // Используем пользовательский цвет
        ),
        child: Text(
          'False',
          style: TextStyle(
            color: Colors.black,
          ),
        ));
  }

  ElevatedButton TrueBtn() {
    return ElevatedButton(
        onPressed: () {
          // Обработка нажатия кнопки "True"
          checkAnswer(true);
        },
        style: ElevatedButton.styleFrom(
          primary: Colors.cyanAccent, // Используем пользовательский цвет
        ),
        child: Text(
          'True',
          style: TextStyle(
            color: Colors.black,
          ),
        ));
  }

  Text questionText() {
    if (questions.isEmpty) {
      return Text(
        'Загрузка вопросов...', // Можно использовать любой текст ожидания
        style: TextStyle(
          fontSize: 24.0,
          fontWeight: FontWeight.bold,
        ),
      );
    } else {
      return Text(
        questions[currentQuestionIndex].text,
        style: TextStyle(
          fontSize: 24.0,
          fontWeight: FontWeight.bold,
        ),
      );
    }
  }
}
