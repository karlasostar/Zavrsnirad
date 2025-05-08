/// Updated version with reordered settings: Font, Font Size, Sound, Sound Type
import 'package:flutter/material.dart';
import 'package:shared_preferences/shared_preferences.dart';

class PagePostavke extends StatefulWidget {
  const PagePostavke({Key? key}) : super(key: key);

  @override
  State<PagePostavke> createState() => _PagePostavkeState();
}

class _PagePostavkeState extends State<PagePostavke> {
  bool _soundEnabled = true;
  double _fontSize = 24;
  String _difficulty = 'Srednje';
  String _fontStyle = 'Default';
  String _soundType = 'Klasični';
  bool _voiceFeedback = false;
  int _taskCount = 10;
  bool _lockedModes = false;

  final List<String> _fontOptions = ['Default', 'Sans', 'Serif', 'Monospace'];
  final List<String> _soundOptions = ['Klasični', 'Zabavni'];
  final List<String> _difficultyLevels = ['Lako', 'Srednje', 'Teško'];

  @override
  void initState() {
    super.initState();
    _loadSettings();
  }

  Future<void> _loadSettings() async {
    SharedPreferences prefs = await SharedPreferences.getInstance();
    setState(() {
      _soundEnabled = prefs.getBool('sound') ?? true;
      _fontSize = prefs.getDouble('fontSize') ?? 24;
      _difficulty = prefs.getString('difficulty') ?? 'Srednje';
      _fontStyle = prefs.getString('fontStyle') ?? 'Default';
      _soundType = prefs.getString('soundType') ?? 'Klasični';
      _voiceFeedback = prefs.getBool('voiceFeedback') ?? false;
      _taskCount = prefs.getInt('taskCount') ?? 10;
      _lockedModes = prefs.getBool('lockedModes') ?? false;
    });
  }

  Future<void> _saveSettings() async {
    SharedPreferences prefs = await SharedPreferences.getInstance();
    prefs.setBool('sound', _soundEnabled);
    prefs.setDouble('fontSize', _fontSize);
    prefs.setString('difficulty', _difficulty);
    prefs.setString('fontStyle', _fontStyle);
    prefs.setString('soundType', _soundType);
    prefs.setBool('voiceFeedback', _voiceFeedback);
    prefs.setInt('taskCount', _taskCount);
    prefs.setBool('lockedModes', _lockedModes);
  }

  Future<void> _resetRecord() async {
    SharedPreferences prefs = await SharedPreferences.getInstance();
    await prefs.setInt('record', 0);
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(content: Text('Rekord je uspješno resetiran.')),
    );
  }

  Future<void> _resetAllSettings() async {
    SharedPreferences prefs = await SharedPreferences.getInstance();
    await prefs.clear();
    await _loadSettings();
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(content: Text('Sve postavke su resetirane.')),
    );
  }

  Widget _dividerWrapper(Widget child) {
    return Container(
      margin: EdgeInsets.symmetric(vertical: 12),
      padding: EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(16),
        boxShadow: [
          BoxShadow(
            color: Colors.black12,
            blurRadius: 6,
            offset: Offset(0, 2),
          )
        ],
        border: Border(
          left: BorderSide(color: Color(0xFFCCCCFF), width: 6),
        ),
      ),
      child: child,
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        backgroundColor: Color(0xFFCCCCFF),
        title: const Text('Postavke', style: TextStyle(color: Color(0xFF440D68), fontWeight: FontWeight.bold)),
        iconTheme: const IconThemeData(color: Color(0xFF440D68)),
        elevation: 0,
      ),
      backgroundColor: const Color(0xFFF8F8E8),
      body: Padding(
        padding: const EdgeInsets.all(16.0),
        child: ListView(
          children: [
            _dividerWrapper(_buildChoiceChips("Font", _fontOptions, _fontStyle, (val) {
              setState(() => _fontStyle = val);
              _saveSettings();
            })),
            _dividerWrapper(Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text('Veličina slova: ${_fontSize.round()}', style: TextStyle(fontWeight: FontWeight.bold)),
                Slider(
                  value: _fontSize,
                  min: 16,
                  max: 36,
                  divisions: 5,
                  label: _fontSize.round().toString(),
                  onChanged: (val) {
                    setState(() => _fontSize = val);
                    _saveSettings();
                  },
                ),
              ],
            )),
            _dividerWrapper(SwitchListTile(
              title: const Text('Zvukovi', style: TextStyle(fontWeight: FontWeight.bold)),
              value: _soundEnabled,
              activeColor: Color(0xFF440D68),
              onChanged: (val) {
                setState(() => _soundEnabled = val);
                _saveSettings();
              },
            )),
            _dividerWrapper(_buildChoiceChips("Vrsta zvuka", _soundOptions, _soundType, (val) {
              setState(() => _soundType = val);
              _saveSettings();
            })),
            _dividerWrapper(SwitchListTile(
              title: const Text('Glasovna potvrda (točno/krivo)', style: TextStyle(fontWeight: FontWeight.bold)),
              value: _voiceFeedback,
              activeColor: Color(0xFF440D68),
              onChanged: (val) {
                setState(() => _voiceFeedback = val);
                _saveSettings();
              },
            )),
            _dividerWrapper(_buildChoiceChips("Težina zadataka", _difficultyLevels, _difficulty, (val) {
              setState(() => _difficulty = val);
              _saveSettings();
            })),
            _dividerWrapper(Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text('Broj zadataka prije kraja: $_taskCount', style: TextStyle(fontWeight: FontWeight.bold)),
                Slider(
                  value: _taskCount.toDouble(),
                  min: 5,
                  max: 15,
                  divisions: 10,
                  label: _taskCount.toString(),
                  onChanged: (val) {
                    setState(() => _taskCount = val.round());
                    _saveSettings();
                  },
                ),
              ],
            )),
            _dividerWrapper(ElevatedButton.icon(
              onPressed: _resetRecord,
              icon: const Icon(Icons.refresh, color: Color(0xFF440D68)),
              label: const Text('Resetiraj rekord', style: TextStyle(color: Color(0xFF440D68), fontWeight: FontWeight.bold)),
              style: ElevatedButton.styleFrom(
                backgroundColor: Color(0xFFCCCCFF),
                padding: const EdgeInsets.symmetric(vertical: 14),
                shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(30)),
              ),
            )),
            _dividerWrapper(ElevatedButton.icon(
              onPressed: _resetAllSettings,
              icon: const Icon(Icons.delete_forever, color: Colors.red),
              label: const Text('Resetiraj sve postavke', style: TextStyle(color: Colors.red, fontWeight: FontWeight.bold)),
              style: ElevatedButton.styleFrom(
                backgroundColor: Colors.white,
                side: BorderSide(color: Colors.red),
                padding: const EdgeInsets.symmetric(vertical: 14),
                shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(30)),
              ),
            )),
          ],
        ),
      ),
    );
  }

  Widget _buildChoiceChips(String label, List<String> options, String selectedValue, Function(String) onSelected) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          label,
          style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold, color: Color(0xFF440D68)),
        ),
        const SizedBox(height: 8),
        Wrap(
          spacing: 10,
          runSpacing: 6,
          children: options.map((option) {
            return ChoiceChip(
              label: Text(option),
              selected: selectedValue == option,
              onSelected: (selected) {
                if (selected) onSelected(option);
              },
              selectedColor: Color(0xFFCCCCFF),
              labelStyle: TextStyle(color: Color(0xFF440D68), fontWeight: FontWeight.w600),
              backgroundColor: Colors.grey.shade100,
              elevation: 2,
              shape: RoundedRectangleBorder(
                borderRadius: BorderRadius.circular(12),
                side: BorderSide(color: Color(0xFF440D68), width: 1),
              ),
            );
          }).toList(),
        ),
      ],
    );
  }
}
