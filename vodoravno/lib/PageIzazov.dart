import 'dart:async';
import 'dart:math';
import 'package:flutter/material.dart';
import 'package:shared_preferences/shared_preferences.dart';
import 'main.dart';
import 'PageTezina.dart';

class PageIzazov extends StatefulWidget {
  @override
  _PageIzazov createState() => _PageIzazov();
}

class _PageIzazov extends State<PageIzazov> {
  Color? _feedbackColor;
  int _firstNumber = 1;
  int _secondNumber = 1;
  String _input = '';
  final Random _random = Random();
  int _score = 0;
  int _record = 0;

  List<int> _availableSecondNumbers = [1, 2, 4]; // Default
  bool _timedMode = false;
  int _timeLeft = 60;
  Timer? _timer;

  @override
  void initState() {
    super.initState();
    _prepareChallenge();
  }

  Future<void> _prepareChallenge() async {
    SharedPreferences prefs = await SharedPreferences.getInstance();
    List<String> selectedSets = prefs.getStringList('numbersSets') ?? ['1,2,4', '5,10', '3,6,9', '7,8'];

    List<int> selectedNumbers = [];
    for (var set in selectedSets) {
      selectedNumbers.addAll(
        set.split(',').map((e) => int.tryParse(e.trim())).whereType<int>(),
      );
    }

    _timedMode = prefs.getBool('timedMode') ?? false;

    if (selectedNumbers.isEmpty) {
      selectedNumbers = [1, 2, 4];
    }

    setState(() {
      _availableSecondNumbers = selectedNumbers;
    });

    await _loadRecord();
    _generateNewChallenge();

    if (_timedMode) {
      _askReadyForTimer(); // <<<<< pozivamo pop-up
    }
  }

  void _askReadyForTimer() {
    showDialog(
      context: context,
      barrierDismissible: false,
      builder: (context) => AlertDialog(
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(20)),
        backgroundColor: Color(0xFFCCCCFF),
        title: Center(child: Text('Spreman za izazov?', style: TextStyle(color: Color(0xFF440D68), fontWeight: FontWeight.bold))),
        content: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            Text('Imaš 1 minutu za riješiti što više zadataka.', style: TextStyle(color: Color(0xFF440D68), fontSize: 22), textAlign: TextAlign.center),
            SizedBox(height: 20),
            Icon(Icons.timer, size: 60, color: Color(0xFF440D68)),
          ],
        ),
        actions: [
          TextButton(
            onPressed: () {
              Navigator.of(context).pop();
              _startTimer(); // pokreće timer
            },
            child: Text('Kreni!', style: TextStyle(color: Color(0xFF440D68), fontSize: 20, fontWeight: FontWeight.bold)),
          ),
          TextButton(
            onPressed: () {
              Navigator.of(context).pop();
              Navigator.push(context, MaterialPageRoute(builder: (context) => PageTezina()));
            },
            child: Text('Postavke', style: TextStyle(color: Colors.red, fontSize: 18, fontWeight: FontWeight.bold)),
          ),
        ],
      ),
    );
  }


  Future<void> _loadRecord() async {
    SharedPreferences prefs = await SharedPreferences.getInstance();
    _record = prefs.getInt('record') ?? 0;
    setState(() {});
  }

  void _updateRecord() async {
    SharedPreferences prefs = await SharedPreferences.getInstance();
    if (_score > _record) {
      await prefs.setInt('record', _score);
    }
  }

  void _startTimer() {
    _timeLeft = 60;
    _timer?.cancel();
    _timer = Timer.periodic(Duration(seconds: 1), (timer) {
      if (_timeLeft > 0) {
        setState(() {
          _timeLeft--;
        });
      } else {
        _timer?.cancel();
        _showTimeUpDialog();
      }
    });
  }

  void _showTimeUpDialog() {
    showDialog(
      context: context,
      barrierDismissible: false,
      builder: (context) => AlertDialog(
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(20)),
        backgroundColor: Color(0xFFCCCCFF),
        title: Center(child: Text('Vrijeme je isteklo!', style: TextStyle(color: Color(0xFF440D68), fontWeight: FontWeight.bold))),
        content: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            Text('Osvojeni bodovi: $_score', style: TextStyle(color: Color(0xFF440D68), fontSize: 24)),
            SizedBox(height: 20),
            Icon(Icons.alarm_off, size: 60, color: Colors.red),
          ],
        ),
        actions: [
          Center(
            child: ElevatedButton(
              onPressed: () {
                Navigator.of(context).pop();
                Navigator.pushReplacement(context, MaterialPageRoute(builder: (context) => MainMenu()));
              },
              style: ElevatedButton.styleFrom(
                backgroundColor: Color(0xFF440D68),
                shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(30)),
              ),
              child: Text('Nazad na izbornik', style: TextStyle(color: Colors.white)),
            ),
          )
        ],
      ),
    );
  }

  void _generateNewChallenge() {
    setState(() {
      _firstNumber = _random.nextInt(10) + 1;
      _secondNumber = _availableSecondNumbers[_random.nextInt(_availableSecondNumbers.length)];
      _input = '';
    });
  }

  void _submitAnswer() {
    if (int.tryParse(_input) == _firstNumber * _secondNumber) {
      setState(() {
        _score++;
        _feedbackColor = Colors.greenAccent;
        if (_score > _record) {
          _record = _score;
          _updateRecord();
        }
      });
      Future.delayed(Duration(milliseconds: 500), () {
        setState(() => _feedbackColor = null);
        _generateNewChallenge();
      });
    } else {
      setState(() {
        _feedbackColor = Colors.redAccent;
        _input = '';
        if (!_timedMode) {
          _score = 0; // Samo ako nije u timed načinu
        }
      });
      Future.delayed(Duration(milliseconds: 500), () {
        setState(() => _feedbackColor = null);
      });
    }
  }


  void _appendInput(String value) {
    setState(() => _input += value);
  }

  void _deleteInput() {
    setState(() {
      if (_input.isNotEmpty) {
        _input = _input.substring(0, _input.length - 1);
      }
    });
  }

  @override
  void dispose() {
    _timer?.cancel();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: Stack(
        children: [
          Positioned.fill(
            child: Container(
              decoration: BoxDecoration(
                image: DecorationImage(
                  image: AssetImage("lib/pictures/background.png"),
                  fit: BoxFit.cover,
                ),
              ),
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
                  Navigator.pushReplacement(context, MaterialPageRoute(builder: (context) => MainMenu()));
                },
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
                onPressed: () async {
                  await Navigator.push(context, MaterialPageRoute(builder: (context) => PageTezina()));
                  _timer?.cancel(); // 🔥 Zaustavi timer

                  await _prepareChallenge(); // 🔥 Učitaj nove postavke
                  setState(() {
                    if (!_timedMode) {
                      _timeLeft = 0; // 🔥 Ako timedMode je isključen, resetiraj prikaz vremena
                    } else {
                      _startTimer(); // 🔥 Ako timedMode ostaje upaljen, pokreni novi timer
                    }
                  });
                },


              ),
            ),
          ),
          if (_timedMode)
            Positioned(
              top: 120,
              right: 20,
              child: Row(
                children: [
                  Icon(Icons.hourglass_bottom, size: 36, color: Color(0xFF440D68)),
                  SizedBox(width: 8),
                  Text('$_timeLeft s', style: TextStyle(fontSize: 28, fontWeight: FontWeight.bold, color: Color(0xFF440D68))),
                ],
              ),
            ),
          Padding(
            padding: const EdgeInsets.only(top: 30.0),
            child: Column(
              children: [
                Text(
                  'IZAZOV',
                  style: TextStyle(fontSize: 80, fontWeight: FontWeight.bold, color: Color(0xFF440D68)),
                ),
                SizedBox(height: 10),
                Container(
                  padding: EdgeInsets.symmetric(horizontal: 24, vertical: 8),
                  decoration: BoxDecoration(
                    color: Color(0xFFCCCCFF),
                    borderRadius: BorderRadius.circular(40),
                  ),
                  child: Text(
                    'Bodovi: $_score  |  Rekord: $_record',
                    style: TextStyle(fontSize: 28, fontWeight: FontWeight.bold, color: Color(0xFF440D68)),
                  ),
                ),
                SizedBox(height: 20),
                Expanded(
                  child: Row(
                    children: [
                      Expanded(
                        flex: 4,
                        child: Center(
                          child: Container(
                            padding: EdgeInsets.all(24),
                            height: 200,
                            decoration: BoxDecoration(
                              color: Color(0xFFCCCCFF),
                              borderRadius: BorderRadius.circular(150),
                            ),
                            child: Row(
                              mainAxisAlignment: MainAxisAlignment.center,
                              children: [
                                Text('$_firstNumber × $_secondNumber = ', style: TextStyle(fontSize: 60, color: Color(0xFF440D68))),
                                Container(
                                  width: 180,
                                  height: 100,
                                  decoration: BoxDecoration(
                                    color: _feedbackColor ?? Colors.white,
                                    borderRadius: BorderRadius.circular(800),
                                  ),
                                  alignment: Alignment.center,
                                  child: Text(
                                    _input,
                                    style: TextStyle(fontSize: 60, color: Color(0xFF440D68)),
                                  ),
                                ),
                              ],
                            ),
                          ),
                        ),
                      ),
                      SizedBox(width: 200),
                      SizedBox(
                        width: 350,
                        height: 750,
                        child: GridView.count(
                          physics: NeverScrollableScrollPhysics(),
                          crossAxisCount: 3,
                          mainAxisSpacing: 5,
                          crossAxisSpacing: 5,
                          childAspectRatio: 1,
                          children: List.generate(12, (index) {
                            if (index == 9) {
                              return _buildButton(Icons.backspace, _deleteInput);
                            } else if (index == 10) {
                              return _buildButtonText('0', () => _appendInput('0'));
                            } else if (index == 11) {
                              return _buildButton(Icons.check, _submitAnswer);
                            } else {
                              return _buildButtonText('${index + 1}', () => _appendInput('${index + 1}'));
                            }
                          }),
                        ),
                      ),
                      SizedBox(width: 50),
                    ],
                  ),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildButton(IconData icon, VoidCallback onPressed) {
    return Center(
      child: SizedBox(
        width: 120,
        height: 120,
        child: ElevatedButton(
          style: ElevatedButton.styleFrom(
            backgroundColor: Color(0xFF440D68),
            shape: CircleBorder(),
            padding: EdgeInsets.all(0),
          ),
          onPressed: onPressed,
          child: Icon(icon, color: Color(0xFFCCCCFF)),
        ),
      ),
    );
  }

  Widget _buildButtonText(String text, VoidCallback onPressed) {
    return Center(
      child: SizedBox(
        width: 120,
        height: 120,
        child: ElevatedButton(
          style: ElevatedButton.styleFrom(
            backgroundColor: Colors.deepPurple.shade100,
            shape: CircleBorder(),
            padding: EdgeInsets.all(0),
          ),
          onPressed: onPressed,
          child: Text(text, style: TextStyle(fontSize: 45, color: Colors.deepPurple)),
        ),
      ),
    );
  }
}
