#include <NeoPixelBus.h>
#include <ESP8266WiFi.h>

const int neoPixelPin = 14;
const int neoPixelCount = 4; //must be at least 4 for this NeoPixelBus
const char *ssid = "Hackland";
const char *password = "hackland1";

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
  while (!Serial); // wait for serial attach
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

NeoPixelBus<NeoGrbFeature, Neo800KbpsMethod> display(neoPixelCount);
void initializeOutputLeds()
{
  Serial.println("Initializing output LEDs...");
  display.Begin();
  display.Show(); // Initialize all pixels to 'off'
}

int step = 0;
RgbColor standbyColour(0, 128, 0);
RgbColor blackColour(0);
void updateOutputStatus()
{
  for (int p = 0; p < neoPixelCount; p++)
  {
    display.SetPixelColor(p, p == step ? standbyColour : blackColour);
  }
  display.Show();
  step++;
  step %= neoPixelCount;
}

void loop()
{
  digitalWrite(LED_BUILTIN, HIGH); // turn the LED on (HIGH is the voltage level)
  delay(250);                      // wait for a second
  digitalWrite(LED_BUILTIN, LOW);  // turn the LED off by making the voltage LOW
  delay(250);                      // wait for a second

  updateOutputStatus();
}
