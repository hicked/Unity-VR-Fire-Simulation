float leftSpeed = 0;
float rightSpeed = 0;

byte LEDPin = 2;
byte leftWheelPin = 3;
byte rightWheelPin = 4;
byte buttonPin = 5;


void setup() {
  Serial.begin(115200);
  pinMode(LEDPin, OUTPUT); // LED
  pinMode(leftWheelPin, INPUT); // Left Wheel
  pinMode(rightWheelPin, INPUT); // Right Wheel
  pinMode(buttonPin, INPUT); // temp button
}

void loop() {
  if (Serial.available()) {
    // READING STUFF FROM UNITY
    String incomingData = Serial.readString();
  }
  else {
    if (!digitalRead(buttonPin)) { // for some reason the current is inverted here?
      leftSpeed = 2;
      rightSpeed = 2;
    }
    else {
      leftSpeed = 0;
      rightSpeed = 0;
    }

    String data = String(leftSpeed) + "," + String(rightSpeed);

    Serial.println(data);
  }
  // leftSpeed = digitalRead(3);
  // rightSpeed = digitalRead(4);
  
  delay(50);
}