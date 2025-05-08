import 'package:flutter/material.dart';

class PageInfo extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: Color(0xFFEFEFFF),
      appBar: AppBar(
        backgroundColor: Color(0xFFCCCCFF),
        title: Text(
          'INFORMACIJE O APLIKACIJI',
          style: TextStyle(color: Color(0xFF440D68)),
        ),
        iconTheme: IconThemeData(color: Color(0xFF440D68)),
      ),
      body: Padding(
        padding: const EdgeInsets.all(40.0),
        child: Center(
          child: Text(
            '''
Dobrodošli u aplikaciju MNOŽILICA!

Ova aplikacija razvijena je kao pomoć djeci u učenju množenja – kroz igru i interaktivne zadatke.

Osim što pomaže djeci, aplikacija omogućuje i roditeljima praćenje napretka svoje djece, dok nastavnici mogu koristiti sadržaj u sklopu nastave kao dodatni edukativni alat.

Aplikacija je razvijena u sklopu završnog rada na Fakultetu elektrotehnike i računarstva (FER).

Hvala što koristite MNOŽILICU i želim Vam uspješno učenje! 
  ''',
            textAlign: TextAlign.center,
            style: TextStyle(
              fontSize: 28,
              fontWeight: FontWeight.w400,
              color: Color(0xFF440D68),
              height: 1.6,
            ),
          ),

        ),
      ),
    );
  }
}
