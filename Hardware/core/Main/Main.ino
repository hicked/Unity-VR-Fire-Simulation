float leftSpeed = 0;
float rightSpeed = 0;

#define L_ENC_A 2
#define L_ENC_B 3
#define R_ENC_A 4
#define R_ENC_B 5

#define buttonPin 6

void setup() {
    Serial.begin(115200);

    pinMode(L_ENC_A, INPUT_PULLUP);
    pinMode(L_ENC_B, INPUT_PULLUP);
    pinMode(R_ENC_A, INPUT_PULLUP);
    pinMode(R_ENC_B, INPUT_PULLUP);

    attachInterrupt(digitalPinToInterrupt(L_ENC_A), readEncoder, CHANGE);
    attachInterrupt(digitalPinToInterrupt(L_ENC_B), readEncoder, CHANGE);
    attachInterrupt(digitalPinToInterrupt(R_ENC_A), readEncoder, CHANGE);
    attachInterrupt(digitalPinToInterrupt(R_ENC_B), readEncoder, CHANGE);

    pinMode(buttonPin, INPUT); // temp button
}


void read_encoder() {
  // Encoder interrupt routine for both pins. Updates counter
  // if they are valid and have rotated a full indent
 
  static uint8_t old_AB = 3;  // Lookup table index
  static int8_t encval = 0;   // Encoder value  
  static const int8_t enc_states[]  = {0,-1,1,0,1,0,0,-1,-1,0,0,1,0,1,-1,0}; // Lookup table

  old_AB <<=2;  // Remember previous state

  if (digitalRead(ENC_A)) old_AB |= 0x02; // Add current state of pin A
  if (digitalRead(ENC_B)) old_AB |= 0x01; // Add current state of pin B
  
  encval += enc_states[( old_AB & 0x0f )];

  // Update counter if encoder has rotated a full indent, that is at least 4 steps
  if( encval > 3 ) {        // Four steps forward
    int changevalue = 1;
    if((micros() - _lastIncReadTime) < _pauseLength) {
      changevalue = _fastIncrement * changevalue; 
    }
    _lastIncReadTime = micros();
    counter = counter + changevalue;              // Update counter
    encval = 0;
  }
  else if( encval < -3 ) {        // Four steps backward
    int changevalue = -1;
    if((micros() - _lastDecReadTime) < _pauseLength) {
      changevalue = _fastIncrement * changevalue; 
    }
    _lastDecReadTime = micros();
    counter = counter + changevalue;              // Update counter
    encval = 0;
  }
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