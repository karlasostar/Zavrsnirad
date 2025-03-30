import 'package:flutter/material.dart';
import 'dart:math';

class IzazovScene extends StatefulWidget {
  const IzazovScene({super.key});

  @override
  State<IzazovScene> createState() => _IzazovSceneState();
}

class _IzazovSceneState extends State<IzazovScene> {
  Random random = Random();
  late int number1;
  late int number2;
  String userInput = "";
  String resultMessage = "";
  int _score = 0;

  @override
  void initState() {
    super.initState();
    _generateQuestion();
  }

  void _generateQuestion() {
    final rand = Random();
    number1 = rand.nextInt(11); // 0 to 10
    number2 = rand.nextInt(11); // 0 to 10
    userInput = "";
    resultMessage = "";
  }

  void _onNumberPress(String value) {
    setState(() {
      if (userInput.length < 4) {
        userInput += value;
      }
    });
  }

  void _onClear() {
    setState(() {
      userInput = "";
      resultMessage = "";
    });
  }

  void _onSubmit() {
    final answer = number1 * number2;
    final input = int.tryParse(userInput);
    setState(() {
      if (input == answer) {
        resultMessage = "Bravo! 🎉";
        _score++;
        _generateQuestion();
      } else {
        resultMessage = "Pokušaj ponovo 😅";
        userInput = '';
      }
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: Colors.lightGreen[100],
      appBar: AppBar(
        title: const Text("IZAZOV"),
        centerTitle: true,
        backgroundColor: Colors.green,
      ),
      body: SingleChildScrollView(
        child: Padding(
          padding: const EdgeInsets.all(16),
          child: Column(
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              Text(
                "Koliko je $number1 × $number2?",
                style: const TextStyle(fontSize: 28, fontWeight: FontWeight.bold),
              ),
              const SizedBox(height: 20),

              Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  // Kružić s brojem točnih odgovora
                  Container(
                    width: 60,
                    height: 60,
                    decoration: const BoxDecoration(
                      color: Colors.white,
                      shape: BoxShape.circle,
                    ),
                    alignment: Alignment.center,
                    child: Text(
                      '$_score',
                      style: const TextStyle(
                        fontSize: 24,
                        fontWeight: FontWeight.bold,
                        color: Colors.black,
                      ),
                    ),
                  ),

                  const SizedBox(width: 16),

                  // Polje za unos broja
                  Expanded(
                    child: Container(
                      padding: const EdgeInsets.symmetric(vertical: 16, horizontal: 32),
                      decoration: BoxDecoration(
                        color: Colors.white,
                        borderRadius: BorderRadius.circular(16),
                      ),
                      child: Text(
                        userInput,
                        style: const TextStyle(fontSize: 32),
                      ),
                    ),
                  ),
                ],
              ),

              const SizedBox(height: 20),

              // Tipkovnica (1-9)
              GridView.builder(
                shrinkWrap: true,
                physics: const NeverScrollableScrollPhysics(),
                gridDelegate: const SliverGridDelegateWithFixedCrossAxisCount(
                  crossAxisCount: 3,
                  mainAxisSpacing: 12,
                  crossAxisSpacing: 12,
                  childAspectRatio: 1, // makes all buttons square
                ),
                itemCount: 9,
                itemBuilder: (context, index) {
                  final number = (index + 1).toString();
                  return _numberButton(number);
                },
              ),

              const SizedBox(height: 12),

              // Red s 0, delete i submit
              GridView.count(
                shrinkWrap: true,
                crossAxisCount: 3,
                mainAxisSpacing: 12,
                crossAxisSpacing: 12,
                childAspectRatio: 1,
                physics: const NeverScrollableScrollPhysics(),
                children: [
                  _actionButton("❌", Colors.red, _onClear),
                  _numberButton("0"),
                  _actionButton("✅", Colors.green, _onSubmit),
                ],
              ),

              const SizedBox(height: 24),

              Text(
                resultMessage,
                style: TextStyle(
                  fontSize: 24,
                  color: resultMessage == "Bravo! 🎉" ? Colors.green[800] : Colors.red[800],
                  fontWeight: FontWeight.bold,
                ),
              ),

              if (resultMessage == "Bravo! 🎉")
                TextButton(
                  onPressed: () {
                    setState(() {
                      _generateQuestion();
                    });
                  },
                  child: const Text(
                    "Novi zadatak",
                    style: TextStyle(fontSize: 18),
                  ),
                )
            ],
          ),
        ),
      ),
    );
  }

  Widget _numberButton(String number) {
    return ElevatedButton(
      onPressed: () => _onNumberPress(number),
      style: ElevatedButton.styleFrom(
        backgroundColor: Colors.orangeAccent,
        foregroundColor: Colors.white,
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
        padding: const EdgeInsets.all(8),
        textStyle: const TextStyle(fontSize: 28),
      ),
      child: Text(number),
    );
  }

  Widget _actionButton(String label, Color color, VoidCallback onPressed) {
    return ElevatedButton(
      onPressed: onPressed,
      style: ElevatedButton.styleFrom(
        backgroundColor: color,
        foregroundColor: Colors.white,
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
        padding: const EdgeInsets.all(8),
        textStyle: const TextStyle(fontSize: 28),
      ),
      child: Text(label),
    );
  }
}
