void setup() {
  Serial.begin(19600);
  pinMode(2, OUTPUT); // LED
  pinMode(3, INPUT); // button
}


const int BUFFER_SIZE = 50; // max size of data being pushed through
char inputData[BUFFER_SIZE];

void loop() {
  if (Serial.available() > 0) {
    int dataLen = Serial.readBytes(inputData, BUFFER_SIZE);

    if (inputData[0] == '0') {
      digitalWrite(2, LOW);
    }
    else if (inputData[0] == '1') {
      digitalWrite(2, HIGH);
    }
  }


  Serial.println(digitalRead(3)); // input from button
  delay(200);
}