#include <Adafruit_NeoPixel.h>
#include <ESP8266WiFi.h>

const char *ssid = "MyRepublic C34D";
const char *password = "mkv2q923t3";
const int neoPixelPin = 14;
const int neoPixelCount = 3;

// the setup function runs once when you press reset or power the board
void setup()
{
  initializeStatusLed();
  initializeSerial();
  initializeWifi();
  initializeOutputLeds();
}
void initializeSerial()
{
  Serial.begin(115200);
  Serial.println();
  Serial.println();
  Serial.println("Initializing serial...");
}

void initializeStatusLed()
{
  Serial.println("Initializing status...");
  // initialize digital pin LED_BUILTIN as an output.
  pinMode(LED_BUILTIN, OUTPUT);
  //hook some neopixels (using 3) to GPIO14 (right side, 8 up from USB, labelled D5)
  pinMode(neoPixelPin, OUTPUT);
}

void initializeWifi()
{
  Serial.println("Initializing Wifi...");
  Serial.print("Connecting to ");
  Serial.print(ssid);
  Serial.print(" ...");
  WiFi.begin(ssid, password); // Connect to the network

  while (WiFi.status() != WL_CONNECTED)
  { // Wait for the Wi-Fi to connect
    delay(1000);
    Serial.print('.');
  }

  Serial.println();
  Serial.println("Connection established!");
  Serial.print("IP: ");
  Serial.println(WiFi.localIP());
}

Adafruit_NeoPixel display;
void initializeOutputLeds()
{
  Serial.println("Initializing output LEDs...");
  display = Adafruit_NeoPixel(neoPixelCount, neoPixelPin, NEO_GRB + NEO_KHZ800);
  display.begin();
  display.show(); // Initialize all pixels to 'off'
}

int step = 0;
void updateOutputStatus()
{
  for (int p = 0; p < neoPixelCount; p++)
  {
    if (step == p)
    {
      display.setPixelColor(p, 0, 128, 0);
    }
    else
    {
      display.setPixelColor(p, 0);
    }
  }
  display.show();
  step++;
  if (step == neoPixelCount)
  {
    step = 0;
  }
}

void loop()
{
  digitalWrite(LED_BUILTIN, HIGH); // turn the LED on (HIGH is the voltage level)
  delay(250);                      // wait for a second
  digitalWrite(LED_BUILTIN, LOW);  // turn the LED off by making the voltage LOW
  delay(250);                      // wait for a second

  updateOutputStatus();
}
