import 'package:flutter/material.dart';
import 'package:fluttertoast/fluttertoast.dart';
import 'package:lab3/Question.dart';

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

class _QuizScreenState extends State<QuizScreen> {
  Map<int, bool> _userAnswers = {};
  List<Question> questions = [
    Question('Столица Франции - Париж?', true),
    Question('Китай находится в Европе?', false),
    Question('Австралия - это остров?', true),
  ];

  int currentQuestionIndex = 0;

  void checkAnswer(bool userAnswer) {
    bool correctAnswer = questions[currentQuestionIndex].answer;
    String message;

    if (userAnswer == correctAnswer) {
      message = 'Правильно!';
    } else {
      message = 'Неправильно!';
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

    _userAnswers[currentQuestionIndex] = userAnswer == correctAnswer;

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
            content: Text("Правильных ответов: $correctAnswersCount\n"
                "Ваша успеваемость: ${score.toStringAsFixed(2)}%"),
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
    return Text(
      questions[currentQuestionIndex].text,
      style: TextStyle(
        fontSize: 24.0,
        fontWeight: FontWeight.bold,
      ),
    );
  }
}
