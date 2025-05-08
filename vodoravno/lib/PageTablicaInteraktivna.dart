import 'dart:math';
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';

class PageTablicaInteraktivna extends StatefulWidget {
  const PageTablicaInteraktivna({Key? key}) : super(key: key);

  @override
  State<PageTablicaInteraktivna> createState() => _PageTablicaInteraktivnaState();
}

class _PageTablicaInteraktivnaState extends State<PageTablicaInteraktivna> {
  final Set<String> _missingKeys = {};
  final Map<String, TextEditingController> _controllers = {};
  final Map<String, Color> _cellColors = {};
  final _random = Random();

  @override
  void initState() {
    super.initState();
    _generateMissingFields();
  }

  void _generateMissingFields() {
    _missingKeys.clear();
    _controllers.clear();
    _cellColors.clear();

    final List<String> allKeys = [];
    for (int row = 1; row <= 10; row++) {
      for (int col = 1; col <= 10; col++) {
        allKeys.add('$row,$col');
      }
    }

    allKeys.shuffle(_random);
    _missingKeys.addAll(allKeys.take(10));

    for (var key in _missingKeys) {
      _controllers[key] = TextEditingController();
      _cellColors[key] = Colors.white;
    }
  }

  void _checkAnswers() {
    setState(() {
      int correctCount = 0;

      for (var key in _missingKeys) {
        final parts = key.split(',');
        final row = int.parse(parts[0]);
        final col = int.parse(parts[1]);
        final correct = row * col;
        final input = int.tryParse(_controllers[key]?.text ?? '');

        if (input == correct) {
          _cellColors[key] = Colors.greenAccent;
          correctCount++;
        } else {
          _cellColors[key] = Colors.redAccent;
        }
      }

      if (correctCount == _missingKeys.length) {
        Future.delayed(Duration(seconds: 1), () {
          setState(() {
            _generateMissingFields();
          });
        });
      }
    });
  }

  @override
  void dispose() {
    for (var controller in _controllers.values) {
      controller.dispose();
    }
    super.dispose();
  }

  Widget _buildHeaderCell(String text) {
    return Container(
      height: 40,
      margin: const EdgeInsets.all(2),
      alignment: Alignment.center,
      decoration: BoxDecoration(
        color: const Color(0xFF440D68),
        borderRadius: BorderRadius.circular(6),
      ),
      child: Text(
        text,
        textAlign: TextAlign.center,
        style: const TextStyle(
          fontSize: 22,
          fontWeight: FontWeight.bold,
          color: Colors.white,
        ),
      ),
    );
  }

  Widget _buildStaticCell(String text) {
    return Container(
      height: 40,
      margin: const EdgeInsets.all(2),
      alignment: Alignment.center,
      decoration: BoxDecoration(
        color: const Color(0xFFCCCCFF),
        borderRadius: BorderRadius.circular(6),
      ),
      child: Text(
        text,
        textAlign: TextAlign.center,
        style: const TextStyle(
          fontSize: 22,
          fontWeight: FontWeight.bold,
          color: Color(0xFF440D68),
        ),
      ),
    );
  }

  Widget _buildInputCell(String key, int row, int col) {
    return Container(
      height: 40,
      margin: const EdgeInsets.all(2),
      decoration: BoxDecoration(
        color: _cellColors[key] ?? Colors.white,
        borderRadius: BorderRadius.circular(6),
      ),
      child: Center(
        child: TextField(
          controller: _controllers[key],
          textAlign: TextAlign.center,
          style: const TextStyle(
            fontSize: 22,
            fontWeight: FontWeight.bold,
            color: Colors.black,
          ),
          decoration: const InputDecoration(
            border: InputBorder.none,
            contentPadding: EdgeInsets.zero,
            isDense: true,
          ),
          keyboardType: const TextInputType.numberWithOptions(signed: false, decimal: false),
          inputFormatters: [FilteringTextInputFormatter.digitsOnly],
        ),
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: const Color(0xFFF8F8E8),
      body: Stack(
        children: [
          // Background
          Positioned.fill(
            child: Image.asset(
              "lib/pictures/background.png",
              fit: BoxFit.cover,
            ),
          ),

          // Back button
          Positioned(
            top: 30,
            left: 20,
            child: CircleAvatar(
              backgroundColor: const Color(0xFFCCCCFF),
              radius: 50,
              child: IconButton(
                icon: const Icon(Icons.arrow_back_rounded, color: Color(0xFF440D68)),
                iconSize: 60,
                onPressed: () => Navigator.pop(context),
              ),
            ),
          ),

          // Title
          Positioned(
            top: 30,
            left: 0,
            right: 0,
            child: Center(
              child: Text(
                'Tablica množenja',
                style: const TextStyle(
                  fontSize: 49,
                  fontWeight: FontWeight.bold,
                  color: Color(0xFF440D68),
                  shadows: [Shadow(color: Colors.black26, offset: Offset(2, 2), blurRadius: 2)],
                ),
              ),
            ),
          ),

          // Table
          Align(
            alignment: Alignment.center,
            child: Padding(
              padding: const EdgeInsets.symmetric(horizontal: 12.0, vertical: 12.0),
              child: Table(
                defaultColumnWidth: const FlexColumnWidth(),
                border: TableBorder.all(color: Colors.transparent),
                children: List.generate(11, (row) {
                  return TableRow(
                    children: List.generate(11, (col) {
                      if (row == 0 && col == 0) {
                        return _buildHeaderCell('×');
                      } else if (row == 0) {
                        return _buildHeaderCell('$col');
                      } else if (col == 0) {
                        return _buildHeaderCell('$row');
                      } else {
                        final key = '$row,$col';
                        if (_missingKeys.contains(key)) {
                          return _buildInputCell(key, row, col);
                        } else {
                          return _buildStaticCell('${row * col}');
                        }
                      }
                    }),
                  );
                }),
              ),
            ),
          ),

          // Confirm button
          Positioned(
            bottom: 30,
            right: 30,
            child: FloatingActionButton(
              onPressed: _checkAnswers,
              backgroundColor: const Color(0xFF440D68),
              child: const Icon(Icons.check, color: Color(0xFFCCCCFF), size: 36),
            ),
          ),
        ],
      ),
    );
  }
}
