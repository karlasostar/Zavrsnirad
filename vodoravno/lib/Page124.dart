// NOVA VERZIJA sa brojacem ponavljanja
import 'dart:math';
import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';
import 'main.dart';
import 'PagePostavke.dart';

class Page124 extends StatefulWidget {
  const Page124({Key? key}) : super(key: key);

  @override
  State<Page124> createState() => _Page124State();
}

class _Page124State extends State<Page124> {
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
      secondNumber = ([1, 2, 4]..shuffle()).first;
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
            top: 40,
            left: 0,
            right: 0,
            child: Center(
              child: Text(
                "Koliko latica se nalazi na ekranu?",
                style: TextStyle(
                  fontSize: 50,
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
                    const SizedBox(height: 60),
                    Expanded(
                      child: Center(

                        child:_buildFlowerRows(total, secondNumber, constraints.maxWidth),
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
          padding: const EdgeInsets.symmetric(horizontal: 60, vertical: 10), // Bigger button
          decoration: BoxDecoration(
            color: const Color(0xFFCCCCFF),
            borderRadius: BorderRadius.circular(100), // Rounded edges
          ),
          child: Center(
            child: Text(
              expression,
              style: TextStyle(
                fontSize: 40,
                fontWeight: FontWeight.bold,
                color: Color(0xFF440D68),
              ),
            ),
          ),
        ),




      );
    }).toList();
  }


  Widget _buildFlowerRows(int total, int secondNumber, double maxWidth) {
    const double spacing = 5.0;
    double broj = 240.0;
    List<List<int>> layout;

    if (total <= 5) {
      layout = [List.generate(total, (i) => i)];
    } else if (total == 6) {
      layout = [List.generate(3, (i) => i), List.generate(3, (i) => i + 3)];
    } else if (total == 7) {
      layout = [List.generate(4, (i) => i), List.generate(3, (i) => i + 4)];
    } else if (total == 8) {
      layout = [List.generate(4, (i) => i), List.generate(4, (i) => i + 4)];
    } else if (total == 9) {
      layout = [List.generate(5, (i) => i), List.generate(4, (i) => i + 4)];
    } else if (total == 10) {
      layout = [List.generate(5, (i) => i), List.generate(5, (i) => i + 5)];
    } else {
      int maxPerRow = min(5, total);
      double width = (maxWidth - spacing * (maxPerRow - 1)) / maxPerRow;

      return Wrap(
        alignment: WrapAlignment.center,
        spacing: spacing,
        runSpacing: spacing,
        children: List.generate(
          total,
              (index) => _flowerImage(secondNumber, width),
        ),
      );
    }
    if (total == 1){
      broj = 350.0;
    } else if (total <= 4){
      broj = 300.0;
    } else if (total == 6){
      broj = 270.0;
    }
    // Calculate width based on longest row (like original logic)
    int maxInRow = layout.map((row) => row.length).reduce(max);
    double imageSize = min(
      broj,
      (maxWidth - ((maxInRow - 1) * spacing)) / maxInRow,
    );


    return Column(
      mainAxisAlignment: MainAxisAlignment.center,
      children: layout.map((row) {
        return Padding(
          padding: const EdgeInsets.symmetric(vertical: spacing / 2),
          child: Row(
            mainAxisAlignment: MainAxisAlignment.center,
            children: row.map((i) {
              return Padding(
                padding: const EdgeInsets.symmetric(horizontal: spacing / 2),
                child: _flowerImage(secondNumber, imageSize),
              );
            }).toList(),
          ),
        );
      }).toList(),
    );
  }



  Widget _flowerImage(int secondNumber, double size) {
    return Image.asset(
      'lib/pictures/${secondNumber}listica.png',
      width: size,
      height: size,
    );
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
