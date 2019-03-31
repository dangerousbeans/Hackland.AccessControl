/*
todo:
- remote unlock request
- figure out how we can make https work, or fix remote server so http works
- fix remote docker server so that api  doesn't redirect to login
- update baseurl once docker server working
- fix DNS entry (talk to Joran)
- Fix doors in admin which are disconnected but still added to people, you can add them. There is no way to get rid of disconnected doors
- Fix bug with scanning an unassigned token and then assigning the token which doesn't unlock
- add caching into the http responses for unlock so at least if api goes down or wifi codes down, the last few swipes will still work offline (remember to clear cache if a swipe does hit api and is disallowed)
- change button pin to 3 (check with Cam)
- verify new wifi reconnect code is working and will jump to alternate access points on connection loss (and come back to the preferred one later?)
*/

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

//from the library manager, search for mcp23008 and install
//then update the .cpp and .h files with the ones from my github fork
//this has modifications to allow the library to work with the esp8266 (set sda scl pins)
//https://github.com/agrath/Adafruit-MCP23008-library
#include <Adafruit_MCP23008.h>

//Required as dependency of Adafruit_MCP23008
#include <Wire.h>

#define SS_PIN 4  //D2
#define RST_PIN 5 //D1
const int RELAY_PIN = D8;
const int REED_PIN = 0;
const int MAGBOND_PIN = 1;
const int BUTTON_PIN = 3;

bool lockReedStatus = false;        //false = magnet present, true = magnet not found
bool lockMagBondStatus = false;     //false = bonded, true = bond not detected
bool lockTriggerStatus = true;      //true = power is on for lock (should be locked), false = power is off for lock (should be unlocked)
bool lockRequestExitStatus = false; //false = button not pressed, true = button pressed for exit
char strBuffer[10];                 //small char buffer for sprintf use

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
const bool debugApiRegister = false;
const bool debugApiValidate = false;
const bool debugRfid = false;
const bool debugFailedReads = false;
const bool debugDoorLockUnlock = true;
const bool debugLockStatus = true;
const bool debugWifi = false;

// the setup function runs once when you press reset or power the board
void setup()
{
  initializeStatusLed();
  initializeSerial();
  if (verifyGpio())
  {
    initializeWifi();
    initializeGpio();
    initializeLockStatus();
    initializeRfidReader();
    initializeApi();
  }
}
int state = LOW;
void loop()
{
  timer.run();
}
bool verifyGpio()
{
  Serial.println("Verifying I2C communication with mcp23008");
  Wire.begin(D3, D4);
  int numberOfDevices = 0;
  for (byte address = 0; address < 127; address++)
  {
    Wire.beginTransmission(address);
    byte error = Wire.endTransmission();
    if (error == 0)
    {
      Serial.print("I2C device found at address 0x");
      if (address < 16)
      {
        Serial.print("0");
      }
      Serial.println(address, HEX);
      numberOfDevices++;
    }
  }
  if (numberOfDevices == 0)
  {
    Serial.println("I2C communication with mcp23008 failed. Aborting startup.");
    digitalWrite(LED_BUILTIN, LOW);
    return false;
  }
  Serial.println("I2C communication with mcp23008 successful");
  return true;
}
void initializeGpio()
{
  mcp.begin(D3, D4);
  mcp.pinMode(REED_PIN, INPUT);
  mcp.pullUp(REED_PIN, HIGH);
  mcp.pinMode(MAGBOND_PIN, INPUT);
  mcp.pullUp(MAGBOND_PIN, HIGH);
  mcp.pinMode(BUTTON_PIN, INPUT);
  pinMode(RELAY_PIN, OUTPUT);
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

  //pinMode(RELAY_PIN, OUTPUT);
}

void initializeLockStatus()
{
  Serial.println(F("Initializing lock status..."));
  lockReedStatus = (mcp.digitalRead(REED_PIN) == LOW);
  lockMagBondStatus = (mcp.digitalRead(MAGBOND_PIN) == LOW);
  lockRequestExitStatus = (mcp.digitalRead(BUTTON_PIN) == HIGH);
  timer.setInterval(200, readLockStatus);
}

int LockReadAttemptNumber = 0;
void readLockStatus()
{
  lockReedStatus = (mcp.digitalRead(REED_PIN) == LOW);
  lockMagBondStatus = (mcp.digitalRead(MAGBOND_PIN) == LOW);
  lockRequestExitStatus = (mcp.digitalRead(BUTTON_PIN) == HIGH);
  if (debugLockStatus)
  {
    Serial.print(F("Reading lock status ("));
    sprintf(strBuffer, "%06d", LockReadAttemptNumber++);
    Serial.print(strBuffer);
    Serial.print(F(") Lock powered "));
    Serial.print(lockTriggerStatus);
    Serial.print(F(" Exit requested "));
    Serial.print(lockRequestExitStatus);
    Serial.print(F(" Reed detects door "));
    Serial.print(lockReedStatus);
    Serial.print(F(" Magnetic bond "));
    Serial.println(lockMagBondStatus);
  }
  if (lockRequestExitStatus && lockTriggerStatus)
  {
    unlockDoor(false);
  }
}

void initializeWifi()
{
  if (debugWifi)
  {
    Serial.println(F("Initializing Wifi..."));
  }

  wifiMulti.addAP("MyRepublic C34D", "mkv2q923t3");
  wifiMulti.addAP("Hackland", "hackland1");
  wifiMulti.addAP("Hackland++ 2G", "hackland1");

  connectWifi();

  timer.setInterval(1000, verifyWifiConnection);
}
void connectWifi()
{
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
void verifyWifiConnection()
{
  if (WiFi.status() != WL_CONNECTED)
  {
    if (debugWifi)
    {
      Serial.println(F("Wifi is not connected..."));
      Serial.println(F("Initializing Wifi..."));
    }
    connectWifi();
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

int RFIDReadAttemptNumber = 0;
void readRfidToSerial()
{
  if (debugRfid)
  {
    Serial.print(F("Read RFID ("));
    sprintf(strBuffer, "%06d", RFIDReadAttemptNumber++);
    Serial.print(strBuffer);
    Serial.println(F(")"));
  }

  if (!mfrc522.PICC_IsNewCardPresent())
  {
    if (debugRfid && debugFailedReads)
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

  String rfidUIDValue = "";
  for (byte i = 0; i < mfrc522.uid.size; i++)
  {
    rfidUIDValue.concat(String(mfrc522.uid.uidByte[i] < 0x10 ? " 0" : " "));
    rfidUIDValue.concat(String(mfrc522.uid.uidByte[i], HEX));
  }
  rfidUIDValue.toUpperCase();

  if (debugRfid)
  {
    Serial.print("UID tag:");
    Serial.println(rfidUIDValue);
  }

  bool shouldUnlock = sendApiValidate(rfidUIDValue);
  if (debugDoorLockUnlock)
  {
    Serial.print("Unlock result:");
    Serial.println(shouldUnlock);
  }
  if (shouldUnlock)
  {
    unlockDoor(false);
  }
}

void unlockDoor(bool permanent)
{
  if (debugDoorLockUnlock)
  {
    Serial.println("Unlock Door");
  }
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
  if (debugDoorLockUnlock)
  {
    Serial.println("Lock Door");
  }
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

bool sendApiRegister()
{
  if (debugApiRegister)
  {
    Serial.println(F("API Register"));
  }

  char url[128];
  if (WiFi.status() != WL_CONNECTED)
  {
    Serial.println(F("Wifi not connected"));
    return false;
  }

  strcpy(url, ApiBaseUrl);
  strcat(url, (const char *)F("door/register"));

  StaticJsonBuffer<300> JsonBuffer;
  JsonObject &JsonEncoder = JsonBuffer.createObject();

  //add properties like this
  //https://techtutorialsx.com/2017/01/08/esp8266-posting-json-data-to-a-flask-server-on-the-cloud/
  JsonEncoder["MacAddress"] = MacAddress;
  JsonEncoder["LockTriggerStatus"] = lockTriggerStatus;
  JsonEncoder["LockReedStatus"] = lockReedStatus;
  JsonEncoder["LockMagBondStatus"] = lockMagBondStatus;
  JsonEncoder["LockRequestExitStatus"] = lockRequestExitStatus;
  //debug
  char JsonMessageBuffer[300];
  JsonEncoder.prettyPrintTo(JsonMessageBuffer, sizeof(JsonMessageBuffer));

  WiFiClient client;
  HTTPClient http;
  if (!http.begin(client, url))
  {
    Serial.println(F("Failed to connect to http server"));
    return false;
  }
  http.addHeader("Content-Type", "application/json");

  if (debugHttp && debugApiRegister)
  {
    Serial.print("Request ");
    Serial.println(url);
    Serial.print("=> ");
    Serial.println(JsonMessageBuffer);
  }

  int httpCode = http.POST(JsonMessageBuffer); //Send the request
  if (httpCode < 0)
  {
    Serial.println(F("Received invalid http response"));
    return false;
  }
  String payload = http.getString(); //Get response

  if (debugHttp && debugApiRegister)
  {
    Serial.print("Response ");
    Serial.println(httpCode); //Print HTTP return code
    Serial.print("<= ");
    Serial.println(payload); //Print request response payload
  }
  http.end(); //Close connection
  return true;
}

bool sendApiValidate(String tokenValue)
{
  if (debugApiValidate)
  {
    Serial.println(F("API Validate"));
    Serial.print("Token value ");
    Serial.println(tokenValue);
  }

  char url[128];
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

  if (WiFi.status() != WL_CONNECTED)
  {
    Serial.println(F("Wifi not connected"));
    return false;
  }

  WiFiClient client;
  HTTPClient http;
  if (!http.begin(client, url))
  {
    Serial.println(F("Failed to connect to http server"));
    return false;
  }
  http.addHeader("Content-Type", "application/json");

  if (debugHttp && debugApiValidate)
  {
    Serial.print("Request ");
    Serial.println(url);
    Serial.print("=> ");
    Serial.println(JsonMessageBuffer);
  }

  int httpCode = http.POST(JsonMessageBuffer); //Send the request
  if (httpCode < 0)
  {
    Serial.println(F("Received invalid http response"));
    return false;
  }
  const char *json = const_cast<char *>(http.getString().c_str());

  if (debugHttp && debugApiValidate)
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

  if (debugApiValidate)
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

    if (debugApiValidate)
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
