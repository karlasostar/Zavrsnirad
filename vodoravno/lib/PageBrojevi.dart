import 'package:flutter/material.dart';
import 'main.dart'; // Ako koristiš za povratak
import 'main.dart';

class PageBrojevi extends StatelessWidget {
  const PageBrojevi({Key? key}) : super(key: key);

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: Stack(
        children: [
          // Pozadina
          Container(
            decoration: BoxDecoration(
              image: DecorationImage(
                image: AssetImage("lib/pictures/background.png"),
                fit: BoxFit.cover,
              ),
            ),
          ),
          // Gumb za povratak
          Positioned(
            top: 30,
            left: 10,
            child: CircleAvatar(
              backgroundColor: Color(0xFFCCCCFF),
              radius: 40,
              child: IconButton(
                icon: Icon(Icons.arrow_back_rounded, color: Color(0xFF440D68)),
                iconSize: 40,
                onPressed: () {
                  Navigator.pop(context);
                },
              ),
            ),
          ),
          // Sadržaj stranice
          Column(
            crossAxisAlignment: CrossAxisAlignment.center,
            children: [
              const SizedBox(height: 40),
              // Naslov gore
              Center(
                child: Text(
                  'Upoznajmo brojeve!',
                  style: TextStyle(
                    fontSize: 40,
                    fontWeight: FontWeight.bold,
                    color: Color(0xFF440D68),
                  ),
                  textAlign: TextAlign.center,
                ),
              ),
              const SizedBox(height: 60),
              // Glavni horizontalni red
              Row(
                mainAxisAlignment: MainAxisAlignment.spaceEvenly,
                crossAxisAlignment: CrossAxisAlignment.center,
                children: [
                  // Lijevo: Broj 3
                  Container(
                    padding: EdgeInsets.symmetric(horizontal: 24, vertical: 16),
                    decoration: BoxDecoration(
                      color: Color(0xFFCCCCFF),
                      borderRadius: BorderRadius.circular(100),
                    ),
                    child: Text(
                      'Broj 3',
                      style: TextStyle(
                        fontSize: 28,
                        color: Color(0xFF440D68),
                        fontWeight: FontWeight.bold,
                      ),
                    ),
                  ),
                  // Sredina: Slika + tekst
                  Column(
                    children: [
                      Image.asset(
                        'lib/pictures/1listica.png',
                        width: 200,
                        height: 200,
                      ),
                      const SizedBox(height: 20),
                      Text(
                        'Cvijet ima tri latice!',
                        style: TextStyle(
                          fontSize: 28,
                          color: Color(0xFF440D68),
                          fontWeight: FontWeight.bold,
                        ),
                        textAlign: TextAlign.center,
                      ),
                    ],
                  ),
                  // Desno: 1x3
                  Container(
                    padding: EdgeInsets.symmetric(horizontal: 24, vertical: 16),
                    decoration: BoxDecoration(
                      color: Color(0xFFCCCCFF),
                      borderRadius: BorderRadius.circular(100),
                    ),
                    child: Text(
                      '1×3',
                      style: TextStyle(
                        fontSize: 28,
                        color: Color(0xFF440D68),
                        fontWeight: FontWeight.bold,
                      ),
                    ),
                  ),
                ],
              ),
            ],
          ),
          // Strelica za dalje (desno dolje)
          Positioned(
            bottom: 30,
            right: 20,
            child: CircleAvatar(
              backgroundColor: Color(0xFFCCCCFF),
              radius: 40,
              child: IconButton(
                icon: Icon(Icons.arrow_forward_rounded, color: Color(0xFF440D68)),
                iconSize: 40,
                onPressed: () {
                  Navigator.push(
                    context,
                    MaterialPageRoute(builder: (context) => MainMenu()), // Ovdje ide tvoja sljedeća stranica!
                  );
                },
              ),
            ),
          ),
        ],
      ),
    );
  }
}
