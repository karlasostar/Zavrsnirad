import 'package:flutter/material.dart';
import 'izazov_scene.dart';  // Import the IzazovScene

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
        child: Stack(
          children: [
            // Place the image on the left side of the screen
            Align(
              alignment: Alignment.bottomCenter,
              child: Padding(
                padding: const EdgeInsets.only(bottom: 25, right: 16), // Adjust the padding for positioning
                child: Image.asset(
                  'lib/pictures/home_4flowers.png', // Path to your image in the pictures folder
                  width: 450, // Set the image size
                  height: 450, // Set the image size
                ),
              ),
            ),
            // Use a Column for the buttons and other elements
            Column(
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
                // Centered "IZAZOV" button
                Center(
                  child: _bigCircleButton(context),  // Pass context to navigate
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }

  // Expanded square button widget
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

  // Circle button widget (larger size)
  Widget _bigCircleButton(BuildContext context) {
    return SizedBox(
      width: 160, // Increase the width to make the button larger
      height: 160, // Increase the height to make the button larger
      child: ElevatedButton(
        onPressed: () {
          // Navigate to IzazovScene when the button is pressed
          Navigator.push(
            context,
            MaterialPageRoute(builder: (context) => const IzazovScene()),
          );
        },
        style: ElevatedButton.styleFrom(
          shape: const CircleBorder(),
        ),
        child: const Text('IZAZOV', style: TextStyle(fontSize: 28)),
      ),
    );
  }
}
