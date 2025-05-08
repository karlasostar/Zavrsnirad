// NOVA VERZIJA sa brojacem ponavljanja
import 'dart:math';
import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';
import 'main.dart';
import 'PagePostavke.dart';

class Page510 extends StatefulWidget {
  const Page510({Key? key}) : super(key: key);

  @override
  State<Page510> createState() => _Page510State();
}

class _Page510State extends State<Page510> {
  late int firstNumber;
  late int secondNumber;
  int counter = 0;

  @override
  void initState() {
    super.initState();
    _generateNewChallenge();
  }

  void _generateNewChallenge() {
    setState(() {
      firstNumber = Random().nextInt(10) + 1;
      secondNumber = ([5,10]..shuffle()).first;
    });
  }

  @override
  Widget build(BuildContext context) {
    int total = firstNumber;
    int maxPerRow = min(5, total);
    double spacing = 8;

    if(maxPerRow >= 5) {
      maxPerRow = 5;
    } else {
      maxPerRow = 5;
    }

    return Scaffold(
      backgroundColor: const Color(0xFFF8F8E8),
      body: Stack(
        children: [
          Positioned.fill(
            child: Image.asset(
              "lib/pictures/background.png",
              fit: BoxFit.cover,
            ),
          ),
          Positioned(
            top: 20,
            left: 20,
            child: CircleAvatar(
              backgroundColor: Color(0xFFCCCCFF),
              radius: 50,
              child: IconButton(
                icon: Icon(Icons.arrow_back_rounded, color: Color(0xFF440D68)),
                iconSize: 60,
                onPressed: () {
                  Navigator.push(context, MaterialPageRoute(builder: (context) => MainMenu()));
                },
              ),
            ),
          ),
          Positioned(
            top: 30,
            left: 0,
            right: 0,
            child: Center(
              child: Text(
                "Koliko latica se nalazi na ekranu?",
                style: TextStyle(
                  fontSize: 40,
                  fontWeight: FontWeight.bold,
                  color: Color(0xFF440D68),
                ),
                textAlign: TextAlign.center,
              ),
            ),
          ),

          Positioned(
            top: 20,
            right: 20,
            child: CircleAvatar(
              backgroundColor: Color(0xFFCCCCFF),
              radius: 50,
              child: IconButton(
                icon: Icon(Icons.settings, color: Color(0xFF440D68)),
                iconSize: 60,
                onPressed: () {
                  Navigator.push(context, MaterialPageRoute(builder: (context) => PagePostavke()));
                },
              ),
            ),
          ),
          LayoutBuilder(
            builder: (context, constraints) {
              double width = (constraints.maxWidth - (spacing * (maxPerRow - 1))) / maxPerRow;

              return Padding(
                padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 40),
                child: Column(
                  children: [
                    Expanded(
                      child: Column(
                        mainAxisAlignment: MainAxisAlignment.start,
                        children: [
                          const SizedBox(height: 60),
                          Wrap(
                            alignment: WrapAlignment.center,
                            spacing: 1,
                            runSpacing: 1,
                            children: List.generate(
                              total,
                                  (index) => Image.asset(
                                'lib/pictures/${secondNumber}listica.png',
                                width: width,
                                height: width,
                              ),
                            ),
                          ),
                        ],
                      ),
                    ),
                    SizedBox(
                      height: 100,
                      child: Row(
                        mainAxisAlignment: MainAxisAlignment.spaceEvenly,
                        children: _generateOptions(context),
                      ),
                    ),
                  ],
                ),
              );
            },
          ),
        ],
      ),
    );
  }

  List<Widget> _generateOptions(BuildContext context) {
    String correctExpression = "$firstNumber×$secondNumber";
    int result = firstNumber * secondNumber;

    List<String> options = [correctExpression];
    while (options.length < 3) {
      int randomA = Random().nextInt(8) + 2;
      int randomB = Random().nextInt(8) + 2;
      String randomExpression = "$randomA×$randomB";
      if (!options.contains(randomExpression) && randomExpression != correctExpression) {
        options.add(randomExpression);
      }
    }
    options.shuffle();

    return options.map((expression) {
      return GestureDetector(
        onTap: () {
          bool isCorrect = expression == correctExpression;
          if (isCorrect) {
            setState(() {
              counter++;
            });
            showDialog(
              context: context,
              barrierDismissible: false,
              builder: (context) {
                Future.delayed(Duration(seconds: 2), () {
                  Navigator.of(context).pop();
                  if (counter < 10) {
                    _generateNewChallenge();
                  } else {
                    showDialog(
                      context: context,
                      builder: (context) {
                        Future.delayed(Duration(seconds: 2), () {
                          Navigator.of(context).pop();
                          Navigator.pushReplacement(
                            context,
                            MaterialPageRoute(builder: (context) => MainMenu()),
                          );
                        });
                        return AlertDialog(
                          shape: RoundedRectangleBorder(
                            borderRadius: BorderRadius.circular(30),
                          ),
                          backgroundColor: Color(0xFFCCCCFF),
                          title: Center(
                            child: Text(
                              "🎉 Bravo!",
                              style: TextStyle(
                                fontSize: 28,
                                fontWeight: FontWeight.bold,
                                color: Color(0xFF440D68),
                              ),
                            ),
                          ),
                          content: Column(
                            mainAxisSize: MainAxisSize.min,
                            children: [
                              SizedBox(height: 10),
                              Text(
                                "Riješio si 10 zadataka!",
                                style: TextStyle(
                                  fontSize: 22,
                                  color: Color(0xFF440D68),
                                ),
                                textAlign: TextAlign.center,
                              ),
                              SizedBox(height: 20),
                              Icon(Icons.star, size: 60, color: Colors.amber),
                            ],
                          ),
                        );
                      },
                    );
                  }
                });
                return _feedbackDialog(true, result);
              },
            );
          } else {
            showDialog(
              context: context,
              barrierDismissible: false,
              builder: (context) {
                Future.delayed(Duration(seconds: 3), () {
                  Navigator.of(context).pop();
                });
                return _feedbackDialog(false, result);
              },
            );
          }
        },
        child: Container(
          padding: const EdgeInsets.symmetric(horizontal: 60, vertical: 30),
          decoration: BoxDecoration(
            color: const Color(0xFFCCCCFF),
            borderRadius: BorderRadius.circular(90),
          ),
          child: LayoutBuilder(
            builder: (context, constraints) {
              return Center(
                child: FittedBox(
                  fit: BoxFit.contain,
                  child: Text(
                    expression,
                    style: TextStyle(
                      fontSize: 100, // large starting font, will scale down as needed
                      fontWeight: FontWeight.bold,
                      color: Color(0xFF440D68),
                    ),
                  ),
                ),
              );
            },
          ),
        ),



      );
    }).toList();
  }

  Widget _feedbackDialog(bool isCorrect, int result) {
    return Dialog(
      shape: RoundedRectangleBorder(
        borderRadius: BorderRadius.circular(30),
      ),
      backgroundColor: Color(0xFFCCCCFF),
      child: Container(
        width: double.infinity,
        height: 300,
        padding: const EdgeInsets.all(24),
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Icon(
              isCorrect ? Icons.check_circle_outline : Icons.cancel_outlined,
              size: 60,
              color: isCorrect ? Color(0xFF228B22) : Color(0xFFD32F2F),
            ),
            SizedBox(height: 20),
            Text(
              isCorrect
                  ? 'TOČNO! $firstNumber × $secondNumber = $result'
                  : 'NETOČNO! Pokušaj ponovno.',
              style: TextStyle(
                fontSize: 28,
                fontWeight: FontWeight.bold,
                color: Color(0xFF440D68),
              ),
              textAlign: TextAlign.center,
            ),
            SizedBox(height: 16),
            Text(
              isCorrect ? 'Odlično!' : 'Ne brini, možeš ti to!',
              style: TextStyle(
                fontSize: 20,
                color: Color(0xFF440D68),
                fontWeight: FontWeight.w500,
              ),
              textAlign: TextAlign.center,
            ),
          ],
        ),
      ),
    );
  }




}
