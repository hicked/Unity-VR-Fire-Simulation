float leftSpeed = 0;
float rightSpeed = 0;

byte LEDPin = 2;
byte leftWheelPin = 3;
byte rightWheelPin = 4;
byte buttonPin = 5;


void setup() {
  Serial.begin(9600);
  pinMode(LEDPin, OUTPUT); // LED
  pinMode(leftWheelPin, INPUT); // Left Wheel
  pinMode(rightWheelPin, INPUT); // Right Wheel
  pinMode(buttonPin, INPUT); // temp button
}

void loop() {
  if (Serial.available() > 0) {

    // READING STUFF FROM UNITY
  }
  // leftSpeed = digitalRead(3);
  // rightSpeed = digitalRead(4);
  // data[0] = leftSpeed;
  // data[1] = rightSpeed;
  if (!digitalRead(buttonPin)) { // for some reason the current is inverted here?
    leftSpeed = 3;
    rightSpeed = 3;
  }
  else {
    leftSpeed = 0;
    rightSpeed = 0;
  }

  String data = String(leftSpeed) + "," + String(rightSpeed);

  // Send the byte array through Serial
  Serial.println(data);


  delay(200);
}