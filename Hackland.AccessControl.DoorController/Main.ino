#include <Adafruit_MCP23008.h>

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
#include <Wire.h>

#define SS_PIN 4  //D2
#define RST_PIN 5 //D1
const int RELAY_PIN = D8;
const int REED_PIN = D3;
const int MAGBOND_PIN = D4;
bool lockReedStatus = false;    //false = magnet present, true = magnet not found
bool lockMagBondStatus = false; //false = bonded, true = bond not detected
bool lockTriggerStatus = true;  //true = power is on for lock (should be locked), false = power is off for lock (should be unlocked)
bool a0State = false;
bool mcpD0State = false;

char strBuffer[10]; //small char buffer for sprintf use

MFRC522 mfrc522(SS_PIN, RST_PIN); // Create MFRC522 instance
ESP8266WiFiMulti wifiMulti;       // Create an instance of the ESP8266WiFiMulti class, called 'wifiMulti'
SimpleTimer timer;                //a timer object to handle periodic tasks without blocking delay()
Adafruit_MCP23008 mcp;

const int ApiRegisterFrequencyMs = 2000;
/*
If in development and utilizing iisexpress, you can use ngrok to allow the nodemcu to contact your local iisexpress
Add a tunnel in ~/.ngrok2/ngrok.yml like so:
accesscontrol:
    addr: localhost:54445
    proto: http
    subdomain: accesscontrol
    host_header: localhost:54445
Then, assuming your port is right, (I use region: au) you should be able to access accesscontrol.au.ngrok.io which will 
route to localhost:54445 on iisexpress correctly
If you don't do this you'll get bad request header errors as iisexpress only allows local requests, so you would need to host this on full-iis for development
Once the tunnel is defined, you'll need to "ngrok start accesscontrol"
Note: i'm using a license for ngrok, if you use the free version you can't customise the subdomain and will get a random one each time
*/
const char ApiBaseUrl[] PROGMEM = "http://accesscontrol.au.ngrok.io/api/";
const bool debugHttp = false;
const bool debugValidate = false;
const bool debugApiRegister = false;
const bool debugRfid = false;
const bool debugLockStatus = true;
const bool debugWifi = true;

// the setup function runs once when you press reset or power the board
void setup()
{
  initializeLockStatus();
  initializeStatusLed();
  initializeSerial();
  initializeGpio();
  initializeWifi();
  initializeRfidReader();
  initializeApi();
}
void initializeGpio()
{
  mcp.pinMode(0, INPUT);
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

void initializeLockStatus()
{
  Serial.println(F("Initializing lock status..."));
  pinMode(REED_PIN, INPUT);
  pinMode(MAGBOND_PIN, INPUT);
  pinMode(A0, INPUT);
  pinMode(D8, INPUT);
  lockReedStatus = (digitalRead(REED_PIN) == HIGH);
  lockMagBondStatus = (digitalRead(MAGBOND_PIN) == HIGH);
  a0State = (digitalRead(A0) == HIGH);
  mcpD0State = (digitalRead(D8) == HIGH);
  timer.setInterval(200, readLockStatus);
}

int LockReadAttemptNumber = 0;
void readLockStatus()
{
  lockReedStatus = (digitalRead(REED_PIN) == HIGH);
  lockMagBondStatus = (digitalRead(MAGBOND_PIN) == HIGH);
  //a0State = (digitalRead(A0) == HIGH);
  mcpD0State = (mcp.digitalRead(0) == HIGH);
  if (debugLockStatus)
  {
    Serial.print(F("Reading lock status ("));
    sprintf(strBuffer, "%06d", LockReadAttemptNumber++);
    Serial.print(strBuffer);
    Serial.print(F(") Lock powered "));
    Serial.print(lockTriggerStatus);
    Serial.print(F(" Reed detects door "));
    Serial.print(lockReedStatus);
    Serial.print(F(" Magnetic bond "));
    Serial.print(lockMagBondStatus);
    //Serial.print(F(" AO "));
    //Serial.print(a0State);
    Serial.print(F(" MCP_D0 "));
    Serial.println(mcpD0State);
  }
}

void initializeWifi()
{
  if (debugWifi)
  {
    Serial.println(F("Initializing Wifi..."));
  }

  wifiMulti.addAP((const char *)F("MyRepublic C34D"), (const char *)F("mkv2q923t3"));
  wifiMulti.addAP((const char *)F("Hackland"), (const char *)F("hackland1"));
  wifiMulti.addAP((const char *)F("Hackland++ 2G"), (const char *)F("hackland1"));
  
  while (wifiMulti.run() != WL_CONNECTED)
  {
    // Wait for the Wi-Fi to connect
    if (debugWifi)
    {
      Serial.print('.');
    }
    delay(1000);
  }

  if (debugWifi)
  {
    Serial.print(F("Connected to "));
    Serial.println(WiFi.SSID()); // Tell us what network we're connected to
    Serial.println(F("Connection established!"));
    Serial.print(F("IP: "));
    Serial.println(WiFi.localIP());
  }
}

void initializeRfidReader()
{
  SPI.begin();        // Init SPI bus
  mfrc522.PCD_Init(); // Init MFRC522
  if (debugRfid)
  {
    mfrc522.PCD_DumpVersionToSerial(); // Show details of PCD - MFRC522 Card Reader details
    Serial.println(F("Scan PICC to see UID, SAK, type, and data blocks..."));
  }
  timer.setInterval(500, readRfidToSerial);
}
void loop()
{
  timer.run();
}

int RFIDReadAttemptNumber = 0;
void readRfidToSerial()
{
  if (debugRfid)
  {
    Serial.print(F("Read RFID ("));
    sprintf(strBuffer, "%06d", RFIDReadAttemptNumber++);
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

  bool unlock = sendApiValidate(content);
  if (unlock)
  {
    unlockDoor(false);
  }
}

void unlockDoor(bool permanent)
{
  digitalWrite(RELAY_PIN, HIGH);
  digitalWrite(LED_BUILTIN, LOW); // turn the LED on (HIGH is the voltage level)
  lockTriggerStatus = false;
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
  lockTriggerStatus = true;
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
  if (debugApiRegister)
  {
    Serial.println(F("API Register"));
  }

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

  if (debugHttp && debugApiRegister)
  {
    Serial.print("Request ");
    Serial.println(url);
    Serial.print("=> ");
    Serial.println(JsonMessageBuffer);
  }

  int httpCode = http.POST(JsonMessageBuffer); //Send the request
  String payload = http.getString();           //Get response

  if (debugHttp && debugApiRegister)
  {
    Serial.print("Response ");
    Serial.println(httpCode); //Print HTTP return code
    Serial.print("<= ");
    Serial.println(payload); //Print request response payload
  }
  http.end(); //Close connection
}

bool sendApiValidate(String tokenValue)
{
  if (debugValidate)
  {
    Serial.println(F("API Validate"));
    Serial.print("Token value ");
    Serial.println(tokenValue);
  }

  char url[64];
  strcpy(url, ApiBaseUrl);
  strcat(url, (const char *)F("door/validate"));

  StaticJsonBuffer<300> JsonBuffer;
  JsonObject &JsonEncoder = JsonBuffer.createObject();

  //add properties like this
  //https://techtutorialsx.com/2017/01/08/esp8266-posting-json-data-to-a-flask-server-on-the-cloud/
  JsonEncoder["MacAddress"] = MacAddress;
  JsonEncoder["TokenValue"] = tokenValue;

  //debug
  char JsonMessageBuffer[300];
  JsonEncoder.prettyPrintTo(JsonMessageBuffer, sizeof(JsonMessageBuffer));

  HTTPClient http;
  http.begin(url);
  http.addHeader("Content-Type", "application/json");

  if (debugHttp && debugValidate)
  {
    Serial.print("Request ");
    Serial.println(url);
    Serial.print("=> ");
    Serial.println(JsonMessageBuffer);
  }

  int httpCode = http.POST(JsonMessageBuffer); //Send the request
  const char *json = const_cast<char *>(http.getString().c_str());

  if (debugHttp && debugValidate)
  {
    Serial.print("Response ");
    Serial.println(httpCode); //Print HTTP return code
    Serial.print("<= ");
    Serial.println(json); //Print request response payload
  }

  const size_t capacity = JSON_OBJECT_SIZE(3) + JSON_OBJECT_SIZE(4) + 130;
  DynamicJsonBuffer jsonBuffer(capacity);
  JsonObject &root = jsonBuffer.parseObject(json);

  bool isUnlockAllowed = root["isUnlockAllowed"]; // true
  const char *message = root["message"];          // "Success"
  int doorReadId = root["doorReadId"];            // 4

  if (debugValidate)
  {

    Serial.print("Response ");
    Serial.print(doorReadId);
    Serial.print(" is allowed ");
    Serial.print(isUnlockAllowed);
    Serial.print(" with message ");
    Serial.println(message);
  }

  JsonObject &matchedPerson = root["matchedPerson"];
  if (matchedPerson.success())
  {
    int matchedPerson_id = matchedPerson["id"];                             // 8
    const char *matchedPerson_emailAddress = matchedPerson["emailAddress"]; // "agrath@gmail.com"
    const char *matchedPerson_name = matchedPerson["name"];                 // "Gareth Evans"

    if (debugValidate)
    {
      Serial.print("Person ");
      Serial.print(matchedPerson_name);
      Serial.print(" (");
      Serial.print(matchedPerson_id);
      Serial.print(") - ");
      Serial.println(matchedPerson_emailAddress);
    }
  }
  http.end(); //Close connection

  return isUnlockAllowed;
}
