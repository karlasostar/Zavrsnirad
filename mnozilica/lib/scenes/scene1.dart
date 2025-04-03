import 'package:flutter/material.dart';

class Scene1 extends StatelessWidget {
  const Scene1({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text("MNOŽENJE 1 2 3"),
        backgroundColor: Colors.green,
      ),
      body: Stack(
        children: [
          // Background container
          Container(
            color: Colors.green[100],
            child: const Center(
              child: Text(
                "Naučimo zajedno",
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
                  _ovalButton("Option 1", Colors.green),
                  _ovalButton("Option 2", Colors.green),
                  _ovalButton("Option 3", Colors.green),
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
