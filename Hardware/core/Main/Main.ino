/* Based on Oleg Mazurov's code for rotary encoder interrupt service routines for AVR micros
   here https://chome.nerpa.tech/mcu/reading-rotary-encoder-on-arduino/
   and using interrupts https://chome.nerpa.tech/mcu/rotary-encoder-interrupt-service-routine-for-avr-micros/

   This example does not use the port read method. Tested with Nano and ESP32
   both encoder A and B pins must be connected to interrupt enabled pins, see here for more info:
   https://www.arduino.cc/reference/en/language/functions/external-interrupts/attachinterrupt/
*/

// Define rotary encoder pins
#define L_ENC_A 6
#define L_ENC_B 7
#define R_ENC_A 2
#define R_ENC_B 3
#define RESOLUTION 20 // how many "clicks" or counts are in one full 360 degree rotation

unsigned long L_lastReadTime = micros();
unsigned long R_lastReadTime = micros(); 

volatile int L_counter = 0;
volatile int R_counter = 0;
float L_rpm = 0.0;
float R_rpm = 0.0;

static uint8_t L_old_AB = 3;  // Lookup table index for left encoder
static uint8_t R_old_AB = 3;  // Lookup table index for right encoder
static int8_t L_encval = 0;   // Encoder value for left encoder
static int8_t R_encval = 0;   // Encoder value for right encoder  
static const int8_t enc_states[]  = {0,-1,1,0,1,0,0,-1,-1,0,0,1,0,1,-1,0}; // Lookup table

void read_L_enc() {
  L_old_AB <<= 2;
  if (digitalRead(L_ENC_A)) L_old_AB |= 0x02;
  if (digitalRead(L_ENC_B)) L_old_AB |= 0x01;

  L_encval += enc_states[(L_old_AB & 0x0f)];
  if (L_encval > 3 || L_encval < -3) {
    int changevalue = (int)(L_encval/3);
    unsigned long deltaT = micros() - L_lastReadTime;
    float L_rpm = (float)(changevalue / RESOLUTION) * 60 / (deltaT / 1000.0);
    L_lastReadTime = micros();
    L_counter += changevalue;
    L_encval = 0;
  }
}

void read_R_enc() {
  R_old_AB <<= 2;
  if (digitalRead(R_ENC_A)) R_old_AB |= 0x02;
  if (digitalRead(R_ENC_B)) R_old_AB |= 0x01;

  R_encval += enc_states[(R_old_AB & 0x0f)];
  if (R_encval > 3 || R_encval < -3) {
    int changevalue = (int)(R_encval/3);
    unsigned long deltaT = micros() - R_lastReadTime;
    float R_rpm = (changevalue / RESOLUTION) * 60 / (deltaT / 1000.0);
    R_lastReadTime = micros();
    R_counter += changevalue;
    R_encval = 0;
  }
}


void setup() {
  // Set encoder pins and attach interrupts
  pinMode(L_ENC_A, INPUT_PULLUP);
  pinMode(L_ENC_B, INPUT_PULLUP);
  pinMode(R_ENC_A, INPUT_PULLUP);
  pinMode(R_ENC_B, INPUT_PULLUP);
  attachInterrupt(digitalPinToInterrupt(L_ENC_A), read_L_enc, CHANGE);
  attachInterrupt(digitalPinToInterrupt(L_ENC_B), read_L_enc, CHANGE);
  attachInterrupt(digitalPinToInterrupt(R_ENC_A), read_R_enc, CHANGE);
  attachInterrupt(digitalPinToInterrupt(R_ENC_B), read_R_enc, CHANGE);

  // Start the serial monitor to show output
  Serial.begin(115200);
}
void loop() {
  if (Serial.available()) {
    // READING STUFF FROM UNITY
    String incomingData = Serial.readString();
  }
  else {
    String data = String(L_counter) + "," + String(R_counter);
    // String x = String(L_rpm) + "," + String(R_rpm);
    //Serial.println(data);
    Serial.println(R_rpm);
    // Serial.println(x);
    

    // String data = String(leftSpeed) + "," + String(rightSpeed);

    // Serial.println(data);
  }
  // leftSpeed = digitalRead(3);
  // rightSpeed = digitalRead(4);
  
  //delay(50); REMOVED DELAY MIGHT BE IMPORTANT FOR THREADS!!!!!!!!!!!!!!!!!!!!!!
}


// float leftSpeed = 0;
// float rightSpeed = 0;

// #define L_ENC_A 2
// #define L_ENC_B 3
// #define R_ENC_A 4
// #define R_ENC_B 5
// #define RESOLUTION 100

// #define buttonPin 6

// unsigned long _lastIncReadTime = micros(); 
// unsigned long _lastDecReadTime = micros(); 
// int _pauseLength = 25000;
// int _fastIncrement = 10;

// volatile int counter = 0;

// // int currentStep = 0;
// // int prev_R_ENC_A;

// void read_encoder() {
//   // Encoder interrupt routine for both pins. Updates counter
//   // if they are valid and have rotated a full indent
 
//   static uint8_t R_old_AB = 3;  // Lookup table index
//   static int8_t R_encval = 0;   // Encoder value  
//   static uint8_t L_old_AB = 3;  // Lookup table index
//   static int8_t L_encval = 0;   // Encoder value
//   static const int8_t enc_states[]  = {0,-1,1,0,1,0,0,-1,-1,0,0,1,0,1,-1,0}; // Lookup table

//   L_old_AB <<=2;  // Remember previous state
//   //R_old_AB <<=2;  // Remember previous state

//   if (digitalRead(L_ENC_A)) L_old_AB |= 0x02; // Add current state of pin A
//   if (digitalRead(L_ENC_B)) L_old_AB |= 0x01; // Add current state of pin B
//   // if (digitalRead(R_ENC_A)) R_old_AB |= 0x02; // Add current state of pin A
//   // if (digitalRead(R_ENC_B)) R_old_AB |= 0x01; // Add current state of pin B
  
//   L_encval += enc_states[( L_old_AB & 0x0f )];
//   // R_encval += enc_states[( R_old_AB & 0x0f )];

//   // Update counter if encoder has rotated a full indent, that is at least 4 steps
//   if( L_encval > 3 ) {        // Four steps forward
//     int changevalue = 1;
//     if((micros() - _lastIncReadTime) < _pauseLength) {
//       changevalue = _fastIncrement * changevalue; 
//     }
//     _lastIncReadTime = micros();
//     counter = counter + changevalue;              // Update counter
//     L_encval = 0;
//   }
//   else if( L_encval < -3 ) {        // Four steps backward
//     int changevalue = -1;
//     if((micros() - _lastDecReadTime) < _pauseLength) {
//       changevalue = _fastIncrement * changevalue; 
//     }
//     _lastDecReadTime = micros();
//     counter = counter + changevalue;              // Update counter
//     L_encval = 0;
//   }
// }

// //   if( R_encval > 3 ) {        // Four steps forward
// //     int changevalue = 1;
// //     if((micros() - _lastIncReadTime) < _pauseLength) {
// //       changevalue = _fastIncrement * changevalue; 
// //     }
// //     _lastIncReadTime = micros();
// //     counter = counter + changevalue;              // Update counter
// //     R_encval = 0;
// //   }
// //   else if( R_encval < -3 ) {        // Four steps backward
// //     int changevalue = -1;
// //     if((micros() - _lastDecReadTime) < _pauseLength) {
// //       changevalue = _fastIncrement * changevalue; 
// //     }
// //     _lastDecReadTime = micros();
// //     counter = counter + changevalue;              // Update counter
// //     encval = 0;
// //   }
// // }


// void setup() {
//     Serial.begin(115200);

//     pinMode(L_ENC_A, INPUT_PULLUP);
//     pinMode(L_ENC_B, INPUT_PULLUP);
//     pinMode(R_ENC_A, INPUT_PULLUP);
//     pinMode(R_ENC_B, INPUT_PULLUP);

//     attachInterrupt(digitalPinToInterrupt(L_ENC_A), read_encoder, CHANGE);
//     attachInterrupt(digitalPinToInterrupt(L_ENC_B), read_encoder, CHANGE);
//     // attachInterrupt(digitalPinToInterrupt(R_ENC_A), read_encoder, CHANGE);
//     // attachInterrupt(digitalPinToInterrupt(R_ENC_B), read_encoder, CHANGE);

//     pinMode(buttonPin, INPUT); // temp button
// }

// void loop() {
//   if (Serial.available()) {
//     // READING STUFF FROM UNITY
//     String incomingData = Serial.readString();
//   }
//   else {
//     read_encoder();
//     Serial.println(counter);
    

//     // String data = String(leftSpeed) + "," + String(rightSpeed);

//     // Serial.println(data);
//   }
//   // leftSpeed = digitalRead(3);
//   // rightSpeed = digitalRead(4);
  
//   delay(50);
// }