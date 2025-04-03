import 'dart:math';
import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';

// Model for flowers
class FlowerModel {
  final int num1;
  final int num2;
  final String imagePath;

  FlowerModel({required this.num1, required this.num2, required this.imagePath});
}

// Function to generate random numbers and flower data
List<FlowerModel> generateRandomFlowerData() {
  Random random = Random();
  int num1 = (random.nextInt(2) + 1) * 5;  // Random num1 from 1, 2, 3
  int num2 = random.nextInt(10) + 1; // Random num2 from 1 to 10

  String imagePath = 'lib/pictures/${num1}listica-removebg-preview.png';

  List<FlowerModel> flowers = [];
  for (int i = 0; i < num2; i++) {
    flowers.add(
      FlowerModel(
        num1: num1,
        num2: num2,
        imagePath: imagePath,
      ),
    );
  }
  return flowers;
}

class Scene4 extends StatelessWidget {
  final List<FlowerModel> flowers = generateRandomFlowerData();

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      // Remove AppBar if not needed
      appBar: AppBar(title: const Text("Multiplication Grid"), backgroundColor: Colors.green,),
      backgroundColor: Colors.green[400],
      body: Column(
        children: [
          // Grid with flower images
          Expanded(
            flex: 9,
            child: Padding(
              padding: const EdgeInsets.all(8.0),
              child: GridView.builder(
                gridDelegate: SliverGridDelegateWithFixedCrossAxisCount(
                  crossAxisCount: _getGridColumns(),
                  crossAxisSpacing: 4,
                  mainAxisSpacing: 4,
                ),
                itemCount: flowers.length,
                itemBuilder: (context, index) {
                  return FlowerTile(flower: flowers[index]);
                },
              ),
            ),
          ),
          // Text between the grid and the buttons
          Padding(
            padding: const EdgeInsets.symmetric(vertical: 16.0),
            child: Text(
              "Prebroji koliko se latica nalazi na slici?", // Example text
              style: TextStyle(
                fontSize: 20,
                fontWeight: FontWeight.bold,
                color: Colors.white,
              ),
            ),
          ),
          // Bottom section with buttons
          Flexible(
            flex: 2, // Smaller area for buttons
            child: Padding(
              padding: const EdgeInsets.all(16.0),
              child: Row(
                mainAxisAlignment: MainAxisAlignment.spaceEvenly,
                children: [
                  Expanded(child: _ovalButton("${flowers.first.num2} x ${flowers.first.num1}")),  // Wrap with Expanded
                  Expanded(child: _ovalButton("${flowers.first.num2 + 1} x ${flowers.first.num1}")),  // Wrap with Expanded
                  Expanded(child: _ovalButton("${flowers.first.num2 + 1} x ${flowers.first.num1 + 1}")),
                ],
              ),
            ),
          ),
        ],
      ),
    );
  }

  // Function to determine the number of grid cells based on num2
  int _getGridColumns() {
    if (flowers.length == 1) {
      return 1;
    } else if (flowers.length == 3 || flowers.length == 2) {
      return 2;
    } else if (flowers.length == 4) {
      return 2;
    } else if (flowers.length == 5) {
      return 3;
    } else if (flowers.length == 6) {
      return 3;
    } else if (flowers.length == 7) {
      return 3;
    } else if (flowers.length == 8 || flowers.length == 9) {
      return 3;
    } else  {
      return 4;
    }
  }

  // Oval button widget
  Widget _ovalButton(String label) {
    return ElevatedButton(
      onPressed: () {
        print("Pressed: $label");
      },
      style: ElevatedButton.styleFrom(
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(30),
        ),
        padding: const EdgeInsets.symmetric(horizontal: 40, vertical: 16),
      ),
      child: Text(label, style: const TextStyle(fontSize: 18)),
    );
  }
}

// Widget for each flower in the grid
class FlowerTile extends StatelessWidget {
  final FlowerModel flower;

  FlowerTile({required this.flower});

  @override
  Widget build(BuildContext context) {
    double width = MediaQuery.of(context).size.width;

    // Get the number of columns in the grid dynamically from the parent widget
    int columns = (context.findAncestorWidgetOfExactType<Scene4>() as Scene4)._getGridColumns();

    // Calculate the image size based on the number of columns and available space
    double imageSize = (width - 30 * 2 - 4 * (columns - 1)) / columns;  // 8 padding (left + right), 4 spacing between items

    return GestureDetector(
      onTap: () {
        print("${flower.num1} x ${flower.num2} = ${flower.num1 * flower.num2}");
      },
      child: Card(
        color: Colors.green[400], // Card background color
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(8)),
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Image.asset(
              flower.imagePath,
              width: imageSize,  // Adjust the image width dynamically based on available space
              height: imageSize, // Keep aspect ratio
              fit: BoxFit.contain, // Ensure image scales without distortion
            ),
          ],
        ),
      ),
    );
  }
}
