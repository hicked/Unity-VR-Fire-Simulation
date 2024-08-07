void setup() {
  Serial.begin(19600);
  pinMode(2, OUTPUT); // LED
  pinMode(3, INPUT); // Left Wheel
  pinMode(4, INPUT); // Right Wheel
}


const int BUFFER_SIZE = 50; // max size of data being pushed through
float leftSpeed = 0;
float rightSpeed = 0;
float data[2] = {0, 0};

void loop() {
  if (Serial.available() > 0) {
    // leftSpeed = digitalRead(3);
    // rightSpeed = digitalRead(4);
    // data[0] = leftSpeed;
    // data[1] = rightSpeed;
    data[0] = 20;
    data[1] = 30;

    String data = String(leftSpeed) + "," + String(rightSpeed);

    // Send the byte array through Serial
    Serial.print(data);

    // if (inputData[0] == '0') {
    //   digitalWrite(2, LOW);
    // }
    // else if (inputData[0] == '1') {
    //   digitalWrite(2, HIGH);
    // }
  }

  delay(200);
}