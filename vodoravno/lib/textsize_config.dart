// text_size_config.dart
import 'package:shared_preferences/shared_preferences.dart';

class text_size_config {
  static Future<double> getFontSize() async {
    SharedPreferences prefs = await SharedPreferences.getInstance();
    return prefs.getDouble('fontSize') ?? 24.0;
  }
}
