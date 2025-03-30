import 'package:flutter/material.dart';

class HomeScene extends StatelessWidget {
  const HomeScene({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Center(
          child: const Text(
            "MNOŽILICA",
            style: TextStyle(fontSize: 24), // Optional: Set a custom font size if needed
          ),
        ),
        backgroundColor: Colors.green, // Optional: Set the app bar to green if needed
      ),

      body: Container(
        color: Colors.green[400],  // Set the background color of the screen to green
        padding: const EdgeInsets.all(16.0),
        child: Column(
          mainAxisAlignment: MainAxisAlignment.spaceEvenly,
          children: [
            // Top row with 2 square buttons stretched
            Row(
              children: [
                _expandedSquareButton("1, 2, 3"),
                const SizedBox(width: 16),
                _expandedSquareButton("5, 10"),
              ],
            ),
            Row(
              children: [
                _expandedSquareButton("4, 6, 9"),
                const SizedBox(width: 16),
                _expandedSquareButton("7, 8"),
              ],
            ),
            Center(
              child: _bigCircleButton("IZAZOV"),
            ),
          ],
        ),
      ),
    );
  }

  Widget _expandedSquareButton(String label) {
    return Expanded(
      child: SizedBox(
        height: 100, // Bigger button height
        child: ElevatedButton(
          onPressed: () {
            // TODO: Handle tap
          },
          style: ElevatedButton.styleFrom(
            shape: RoundedRectangleBorder(
              borderRadius: BorderRadius.zero, // force square look
            ),
          ),
          child: Text(label, style: const TextStyle(fontSize: 24)),
        ),
      ),
    );
  }

  Widget _bigCircleButton(String label) {
    return SizedBox(
      width: 150,
      height: 120,
      child: ElevatedButton(
        onPressed: () {
          // TODO: Handle tap
        },
        style: ElevatedButton.styleFrom(
          shape: const CircleBorder(),
        ),
        child: Text(label, style: const TextStyle(fontSize: 28)),
      ),
    );
  }
}
