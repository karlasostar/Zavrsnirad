import 'package:flutter/material.dart';
import 'izazov_scene.dart';  // Import the IzazovScene
import 'scene1.dart';  // Import Scene 1
import 'scene2.dart';  // Import Scene 2
import 'scene3.dart';  // Import Scene 3
import 'scene4.dart';  // Import Scene 4

class HomeScene extends StatelessWidget {
  const HomeScene({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Center(
          child: Text(
            "MNOŽILICA",
            style: TextStyle(fontSize: 24),
          ),
        ),
        backgroundColor: Colors.green,
      ),
      body: Container(
        color: Colors.green[400],
        padding: const EdgeInsets.all(16.0),
        child: Stack(
          children: [
            Align(
              alignment: Alignment.bottomCenter,
              child: Padding(
                padding: const EdgeInsets.only(bottom: 25, right: 16),
                child: Image.asset(
                  'lib/pictures/home_4flowers.png',
                  width: 800,
                  height: 520,
                ),
              ),
            ),
            Column(
              mainAxisAlignment: MainAxisAlignment.spaceEvenly,
              children: [
                Row(
                  children: [
                    _expandedSquareButton(context, "1, 2, 3", const Scene1()),
                    const SizedBox(width: 16),
                    _expandedSquareButton(context, "5, 10",  Scene2()),
                  ],
                ),
                Row(
                  children: [
                    _expandedSquareButton(context, "4, 6, 9", const Scene3()),
                    const SizedBox(width: 16),
                    _expandedSquareButton(context, "7, 8", Scene4()),
                  ],
                ),
                Center(
                  child: _bigCircleButton(context),
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }

  Widget _expandedSquareButton(BuildContext context, String label, Widget scene) {
    return Expanded(
      child: SizedBox(
        height: 100,
        child: ElevatedButton(
          onPressed: () {
            Navigator.push(
              context,
              MaterialPageRoute(builder: (context) => scene),
            );
          },
          style: ElevatedButton.styleFrom(
            shape: RoundedRectangleBorder(
              borderRadius: BorderRadius.zero,
            ),
          ),
          child: Text(label, style: const TextStyle(fontSize: 24)),
        ),
      ),
    );
  }

  Widget _bigCircleButton(BuildContext context) {
    return SizedBox(
      width: 160,
      height: 160,
      child: ElevatedButton(
        onPressed: () {
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
