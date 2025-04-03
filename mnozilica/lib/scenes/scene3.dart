import 'package:flutter/material.dart';

class Scene3 extends StatelessWidget {
  const Scene3({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text("Scene 1"),
        backgroundColor: Colors.green,
      ),
      body: Stack(
        children: [
          // Background container
          Container(
            color: Colors.green[100],
            child: const Center(
              child: Text(
                "Welcome to Scene 1",
                style: TextStyle(fontSize: 24, fontWeight: FontWeight.bold),
              ),
            ),
          ),
          // Positioned buttons at the bottom
          Align(
            alignment: Alignment.bottomCenter,
            child: Padding(
              padding: const EdgeInsets.only(bottom: 20),
              child: Row(
                mainAxisAlignment: MainAxisAlignment.spaceEvenly,
                children: [
                  _ovalButton("Option 1", Colors.red),
                  _ovalButton("Option 2", Colors.blue),
                  _ovalButton("Option 3", Colors.orange),
                ],
              ),
            ),
          ),
        ],
      ),
    );
  }

  Widget _ovalButton(String label, Color color) {
    return SizedBox(
      width: 120,
      height: 60,
      child: ElevatedButton(
        onPressed: () {
          // TODO: Add functionality
        },
        style: ElevatedButton.styleFrom(
          backgroundColor: color,
          shape: RoundedRectangleBorder(
            borderRadius: BorderRadius.circular(30), // Oval shape
          ),
        ),
        child: Text(label, style: const TextStyle(fontSize: 18, color: Colors.white)),
      ),
    );
  }
}
