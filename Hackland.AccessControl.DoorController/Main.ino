#include <ESP8266WiFi.h>
#include <SPI.h>
#include <MFRC522.h>

const char *ssid = "Hackland";
const char *password = "hackland1";

#define SS_PIN 4
#define RST_PIN 5
int RELAY_PIN = D8;

MFRC522 mfrc522(SS_PIN, RST_PIN); // Create MFRC522 instance

// the setup function runs once when you press reset or power the board
void setup()
{
  initializeStatusLed();
  initializeSerial();
  initializeWifi();
  initializeRfidReader();
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
  Serial.print(F("Connecting to "));
  Serial.print(ssid);
  Serial.print(" ...");
  WiFi.begin(ssid, password); // Connect to the network

  while (WiFi.status() != WL_CONNECTED)
  { // Wait for the Wi-Fi to connect
    delay(1000);
    Serial.print('.');
  }

  Serial.println();
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
}
void loop()
{
  readRfidToSerial();
  delay(1000);
}

int attempt = 0;
void readRfidToSerial()
{
  Serial.print(F("Read RFID ("));
  Serial.print(attempt++);
  Serial.println(F(")"));

  if (!mfrc522.PICC_IsNewCardPresent())
  {
    Serial.println(F("No card present"));

    return;
  }

  // Select one of the cards
  if (!mfrc522.PICC_ReadCardSerial())
  {
    Serial.println(F("Failed to select card"));

    return;
  }

  Serial.println(F("Card detected"));

  // Dump debug info about the card; PICC_HaltA() is automatically called
  //mfrc522.PICC_DumpToSerial(&(mfrc522.uid));

  Serial.print("UID tag:");
  String content = "";
  for (byte i = 0; i < mfrc522.uid.size; i++)
  {
    content.concat(String(mfrc522.uid.uidByte[i] < 0x10 ? " 0" : " "));
    content.concat(String(mfrc522.uid.uidByte[i], HEX));
  }
  content.toUpperCase();
  Serial.println(content);

  digitalWrite(RELAY_PIN, HIGH);
  digitalWrite(LED_BUILTIN, LOW); // turn the LED on (HIGH is the voltage level)

  delay(5000);
  digitalWrite(RELAY_PIN, LOW);
  digitalWrite(LED_BUILTIN, HIGH);  // turn the LED off by making the voltage LOW
}
