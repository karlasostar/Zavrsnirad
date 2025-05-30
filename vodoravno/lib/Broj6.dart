import 'package:flutter/material.dart';
import 'Broj3.dart';
import 'Broj7.dart';
import 'main.dart';

class Broj6 extends StatelessWidget {
  const Broj6({Key? key}) : super(key: key);

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: Stack(
        children: [
          // Pozadina
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
                  Navigator.pop(context);
                },
              ),
            ),
          ),

          // Gumb za postavke (isti kao u Page124)
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
                  Navigator.push(context, MaterialPageRoute(builder: (context) => MainMenu()));
                },
              ),
            ),
          ),

          // Naslov poravnat s gumbima
          Positioned(
            top: 30,
            left: 0,
            right: 0,
            child: Center(
              child: Text(
                'Upoznajmo brojeve!',
                style: TextStyle(
                  fontSize: 49,
                  fontWeight: FontWeight.bold,
                  color: Color(0xFF440D68),
                  shadows: [
                    Shadow(
                      color: Colors.black26,
                      offset: Offset(2, 2),
                      blurRadius: 2,
                    )
                  ],
                ),
                textAlign: TextAlign.center,
              ),
            ),
          ),

          // Sadržaj stranice
          Column(
            crossAxisAlignment: CrossAxisAlignment.center,
            children: [
              const SizedBox(height: 140),
              // Glavni red
              Expanded(
                child: Padding(
                  padding: const EdgeInsets.symmetric(horizontal: 16.0),
                  child: Row(
                    mainAxisAlignment: MainAxisAlignment.spaceEvenly,
                    crossAxisAlignment: CrossAxisAlignment.center,
                    children: [
                      // Lijevo: Broj
                      _decoratedBoxText("Broj 6", 42),

                      // Sredina: Slika + opis
                      Column(
                        mainAxisAlignment: MainAxisAlignment.center,
                        children: [
                          Image.asset(
                            'lib/pictures/6listica.png',
                            width: 320,
                            height: 320,
                            fit: BoxFit.contain,
                          ),
                          const SizedBox(height: 16),
                          Text(
                            'Cvijet ima šest latica!',
                            style: TextStyle(
                              fontSize: 40,
                              fontWeight: FontWeight.bold,
                              color: Color(0xFF440D68),
                            ),
                            textAlign: TextAlign.center,
                          ),
                        ],
                      ),

                      // Desno: Množenje
                      _decoratedBoxText("1×6", 42),
                    ],
                  ),
                ),
              ),
            ],
          ),

          // Strelica za dalje
          Positioned(
            bottom: 30,
            right: 20,
            child: CircleAvatar(
              backgroundColor: Color(0xFFCCCCFF),
              radius: 50,
              child: IconButton(
                icon: Icon(Icons.arrow_forward_rounded, color: Color(0xFF440D68)),
                iconSize: 40,
                onPressed: () {
                  Navigator.push(
                    context,
                    MaterialPageRoute(builder: (context) => Broj7()),
                  );
                },
              ),
            ),
          ),
        ],
      ),
    );
  }

  // Komponenta za zaobljeni ljubičasti box s tekstom
  Widget _decoratedBoxText(String text, double fontSize) {
    return Container(
      padding: EdgeInsets.symmetric(horizontal: 32, vertical: 22),
      decoration: BoxDecoration(
        color: Color(0xFFCCCCFF),
        borderRadius: BorderRadius.circular(100),
        boxShadow: [
          BoxShadow(
            color: Colors.black26,
            offset: Offset(2, 4),
            blurRadius: 5,
          ),
        ],
      ),
      child: Text(
        text,
        style: TextStyle(
          fontSize: fontSize,
          color: Color(0xFF440D68),
          fontWeight: FontWeight.bold,
        ),
      ),
    );
  }
}
