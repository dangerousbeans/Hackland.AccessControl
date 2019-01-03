# Hackland.AccessControl

Access control system for the Auckland Hackland hack space.

## Getting Started

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes. 

### Prerequisites

* [VSCode](https://code.visualstudio.com/) - IDE
* [Arduino](https://www.arduino.cc/en/Main/Software) - Compiler
* [ESP8266](https://github.com/esp8266/Arduino) - Board support
* Hardware (see below)

### Hardware

You will need the following:
(where (s) is shown, you would multiply this for each door)
 
* Commercial maglock(s)
* NodeMCU
* 5V power supply
* 12V battery
* 12V door power supply & charger; this has back EMF protection and charges a 12V battery and mains input
* Cable
* Emergency exit button(s)
* Request exit button(s)
* Arduino compatible RFID reader(s)
* 12V relay(s)

How all of this is connected is left until I can set my local up and document this.

### Installing

Please note: I have committed my .vscode folder to make this project more portable between my own machines. You may need to merge or tweak files in this folder.

1) Install visual studio code
2) Install arduino
3) Start visual studio code
4) Install the arduino plugin from Microsoft in extensions
5) Go to file->properties->settings then arduino settings, click the link to get the JSON editor
6) Paste the following over (or merge) into the settings.json

```
{
    "arduino.path": "C:\\Program Files (x86)\\Arduino",
    "arduino.additionalUrls": "http://arduino.esp8266.com/stable/package_esp8266com_index.json",
    "arduino.logLevel": "info",
    "arduino.enableUSBDetection": true, 
    "C_Cpp.intelliSenseEngine": "Tag Parser"
}
```

8) Start arduino, go into settings and make sure that additionalUrls is populated with the esp URL from above, then go to boards manager and search for nodemcu. Install esp8266 by ESP8266 Community
9) Restart vscode, and open the Hackland.AccessControl.DoorController folder from the repo 
10) Choose NodeMCU 1.0 (ESP-12E Module) as the board in the bar at the bottom
11) Change the wifi ssid and password in the sketch near the top to your ones
12) Edit the c_cpp_properties.json file and update the paths to match your environment, restart vscode again
13) Hit ctrl+shift+p and type "Arduino: Upload" without the quotes, this will build and upload the blink sketch
14) To make builds faster, edit arduino.json in .vscode folder and add "output": "../build" as a setting

## Contributing

No published code of conduct, pull requests reviewed and accepted

## Versioning

No versioning scheme currently defined

## Authors

* **Gareth Evans** - *Initial work* - [agrath](https://github.com/agrath)

See also the list of [contributors](https://github.com/agrath/Hackland.AccessControl/graphs/contributors) who participated in this project.

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details


