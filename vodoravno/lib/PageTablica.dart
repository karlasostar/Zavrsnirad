import 'package:flutter/material.dart';

import 'PageTablicaInteraktivna.dart';


class PageTablica extends StatelessWidget {
  const PageTablica({Key? key}) : super(key: key);

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: const Color(0xFFF8F8E8),
      body: Stack(
        children: [
          // Pozadina
          Positioned.fill(
            child: Image.asset(
              "lib/pictures/background.png",
              fit: BoxFit.cover,
            ),
          ),

          // Naslov i gumb za povratak
          Positioned(
            top: 30,
            left: 20,
            child: CircleAvatar(
              backgroundColor: const Color(0xFFCCCCFF),
              radius: 50,
              child: IconButton(
                icon: const Icon(Icons.arrow_back_rounded, color: Color(0xFF440D68)),
                iconSize: 60,
                onPressed: () {
                  Navigator.pop(context);
                },
              ),
            ),
          ),

          // Naslov
          Positioned(
            top: 30,
            left: 0,
            right: 0,
            child: Center(
              child: Text(
                'Tablica množenja',
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

          // Tablica
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
                        return _buildCell('${row * col}');
                      }
                    }),
                  );
                }),
              ),
            ),
          ),

          // Gumb "Provježbaj"
          Positioned(
            bottom: 30,
            right: 30,
            child: ElevatedButton.icon(
              onPressed: () {
                Navigator.push(
                  context,
                  MaterialPageRoute(builder: (context) => const PageTablicaInteraktivna()),
                );
              },
              icon: Icon(Icons.edit, color: Color(0xFF440D68)),
              label: Text(
                'Provježbaj',
                style: TextStyle(
                  fontSize: 22,
                  fontWeight: FontWeight.bold,
                  color: Color(0xFF440D68),
                ),
              ),
              style: ElevatedButton.styleFrom(
                backgroundColor: Color(0xFFCCCCFF),
                shape: RoundedRectangleBorder(
                  borderRadius: BorderRadius.circular(30),
                ),
                padding: EdgeInsets.symmetric(horizontal: 24, vertical: 14),
              ),
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildHeaderCell(String text) {
    return Container(
      margin: EdgeInsets.all(2),
      alignment: Alignment.center,
      decoration: BoxDecoration(
        color: Color(0xFF440D68),
        borderRadius: BorderRadius.circular(6),
      ),
      child: FittedBox(
        fit: BoxFit.scaleDown,
        child: Text(
          text,
          style: TextStyle(
            fontSize: 22,
            fontWeight: FontWeight.bold,
            color: Colors.white,
          ),
        ),
      ),
    );
  }

  Widget _buildCell(String text) {
    return Container(
      margin: EdgeInsets.all(2),
      alignment: Alignment.center,
      decoration: BoxDecoration(
        color: Color(0xFFCCCCFF),
        borderRadius: BorderRadius.circular(6),
      ),
      child: FittedBox(
        fit: BoxFit.scaleDown,
        child: Text(
          text,
          style: TextStyle(
            fontSize: 22,
            fontWeight: FontWeight.bold,
            color: Color(0xFF440D68),
          ),
        ),
      ),
    );
  }
}
