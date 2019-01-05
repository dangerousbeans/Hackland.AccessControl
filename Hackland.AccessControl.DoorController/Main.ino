#include <ESP8266WiFi.h>
#include <SPI.h>

#include <ESP8266WiFiMulti.h> //Wi-Fi-Multi library
#include <ESP8266HTTPClient.h>

//The following libraries must be installed into your arduino libraries folder

//from the library manager, search for frc and install MFRC522 By GithubCommunity Version 1.4.3
#include <MFRC522.h>
//from the library manager, search for ArduinoJson and install from Benoit Blanchon, select Version 5.13.3 (beta 6.x.x won't work)
#include <ArduinoJson.h>
//from github, https://github.com/schinken/SimpleTimer, download the cpp and h files (raw) and place into a folder named "SimpleTimer" in the arduino libraries folder
#include <SimpleTimer.h>

#define SS_PIN 4
#define RST_PIN 5
const int RELAY_PIN = D8;
const bool debugRfid = true;

MFRC522 mfrc522(SS_PIN, RST_PIN); // Create MFRC522 instance
ESP8266WiFiMulti wifiMulti;       // Create an instance of the ESP8266WiFiMulti class, called 'wifiMulti'
SimpleTimer timer;                //a timer object to handle periodic tasks without blocking delay()

const int ApiRegisterFrequencyMs = 2000;
const char ApiBaseUrl[] PROGMEM = "http://10.0.0.116/api/";
const bool debugHttp = true;

// the setup function runs once when you press reset or power the board
void setup()
{
  initializeStatusLed();
  initializeSerial();
  initializeWifi();
  initializeRfidReader();
  initializeApi();
}
void initializeSerial()
{
  Serial.begin(115200);
  while (!Serial)
  {
    ; // wait for serial attach
  }
  Serial.println();
  Serial.println();
  Serial.println(F("Initializing serial..."));
}

void initializeStatusLed()
{
  Serial.println(F("Initializing status..."));
  pinMode(LED_BUILTIN, OUTPUT);
  digitalWrite(LED_BUILTIN, HIGH);

  pinMode(RELAY_PIN, OUTPUT);
}

void initializeWifi()
{
  Serial.println(F("Initializing Wifi..."));
  wifiMulti.addAP((const char *)F("MyRepublic C34D"), (const char *)F("mkv2q923t3"));
  wifiMulti.addAP((const char *)F("Hackland"), (const char *)F("hackland1"));
  while (wifiMulti.run() != WL_CONNECTED)
  {
    // Wait for the Wi-Fi to connect
    Serial.print('.');
    delay(1000);
  }

  Serial.print(F("Connected to "));
  Serial.println(WiFi.SSID()); // Tell us what network we're connected to
  Serial.println(F("Connection established!"));
  Serial.print(F("IP: "));
  Serial.println(WiFi.localIP());
}

void initializeRfidReader()
{
  SPI.begin();                       // Init SPI bus
  mfrc522.PCD_Init();                // Init MFRC522
  mfrc522.PCD_DumpVersionToSerial(); // Show details of PCD - MFRC522 Card Reader details
  Serial.println(F("Scan PICC to see UID, SAK, type, and data blocks..."));
  timer.setInterval(500, readRfidToSerial);
}
void loop()
{
  timer.run();
}

int attempt = 0;
void readRfidToSerial()
{
  if (debugRfid)
  {
    Serial.print(F("Read RFID ("));
    Serial.print(attempt++);
    Serial.println(F(")"));
  }

  if (!mfrc522.PICC_IsNewCardPresent())
  {
    if (debugRfid)
    {
      Serial.println(F("No card present"));
    }

    return;
  }

  // Select one of the cards
  if (!mfrc522.PICC_ReadCardSerial())
  {
    if (debugRfid)
    {
      Serial.println(F("Failed to select card"));
    }

    return;
  }

  if (debugRfid)
  {
    Serial.println(F("Card detected"));
  }

  // Dump debug info about the card; PICC_HaltA() is automatically called
  //mfrc522.PICC_DumpToSerial(&(mfrc522.uid));

  String content = "";
  for (byte i = 0; i < mfrc522.uid.size; i++)
  {
    content.concat(String(mfrc522.uid.uidByte[i] < 0x10 ? " 0" : " "));
    content.concat(String(mfrc522.uid.uidByte[i], HEX));
  }
  content.toUpperCase();

  if (debugRfid)
  {
    Serial.print("UID tag:");
    Serial.println(content);
  }
}

void unlockDoor(bool permanent)
{
  digitalWrite(RELAY_PIN, HIGH);
  digitalWrite(LED_BUILTIN, LOW); // turn the LED on (HIGH is the voltage level)

  if (!permanent)
  {
    //call lockDoor in 5 sec
    timer.setTimeout(5000, lockDoor);
  }
}

void lockDoor()
{
  digitalWrite(RELAY_PIN, LOW);
  digitalWrite(LED_BUILTIN, HIGH); // turn the LED off by making the voltage LOW
}

uint8_t MacAddressBytes[6];
char MacAddress[18];
//https://community.blynk.cc/t/solved-unique-mac-identification-of-a-nodemcu/15915/5
void initializeApi()
{
  Serial.println(F("Initializing API..."));
  WiFi.macAddress(MacAddressBytes);
  for (int i = 0; i < sizeof(MacAddressBytes); ++i)
  {
    if (i < sizeof(MacAddressBytes) - 1)
    {
      sprintf(MacAddress, "%s%02x:", MacAddress, MacAddressBytes[i]);
    }
    else
    {
      sprintf(MacAddress, "%s%02x", MacAddress, MacAddressBytes[i]);
    }
  }
  Serial.print("Device mac address: ");
  Serial.println(MacAddress);

  timer.setInterval(ApiRegisterFrequencyMs, sendApiRegister);
}

void sendApiRegister()
{
  Serial.println(F("API Register"));

  char url[64];
  strcpy(url, ApiBaseUrl);
  strcat(url, (const char *)F("door/register"));

  StaticJsonBuffer<300> JsonBuffer;
  JsonObject &JsonEncoder = JsonBuffer.createObject();

  //add properties like this
  //https://techtutorialsx.com/2017/01/08/esp8266-posting-json-data-to-a-flask-server-on-the-cloud/
  JsonEncoder["MacAddress"] = MacAddress;

  //debug
  char JsonMessageBuffer[300];
  JsonEncoder.prettyPrintTo(JsonMessageBuffer, sizeof(JsonMessageBuffer));

  HTTPClient http;
  http.begin(url);
  http.addHeader("Content-Type", "application/json");

  if (debugHttp)
  {
    Serial.print("Request ");
    Serial.println(url);
    Serial.print("=> ");
    Serial.println(JsonMessageBuffer);
  }

  int httpCode = http.POST(JsonMessageBuffer); //Send the request
  String payload = http.getString();           //Get response

  if (debugHttp)
  {
    Serial.print("Response ");
    Serial.println(httpCode); //Print HTTP return code
    Serial.print("<= ");
    Serial.println(payload); //Print request response payload
  }
  http.end(); //Close connection
}
