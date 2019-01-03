#include <ESP8266WiFi.h>

const char *ssid = "MyRepublic C34D";
const char *password = "mkv2q923t3";

// the setup function runs once when you press reset or power the board
void setup()
{
  initializeStatusLed();

  initializeSerial();
  initializeWifi();
}
void initializeSerial()
{
  Serial.begin(115200);
  Serial.println();
  Serial.println();
}
void initializeStatusLed()
{
  // initialize digital pin LED_BUILTIN as an output.
  pinMode(LED_BUILTIN, OUTPUT);
}
void initializeWifi()
{
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
// the loop function runs over and over again forever
void loop()
{
  digitalWrite(LED_BUILTIN, HIGH); // turn the LED on (HIGH is the voltage level)
  delay(250);                      // wait for a second
  digitalWrite(LED_BUILTIN, LOW);  // turn the LED off by making the voltage LOW
  delay(250);                      // wait for a second
}
