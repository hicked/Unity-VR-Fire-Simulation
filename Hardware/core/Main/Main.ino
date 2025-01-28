// #include <FastLED.h>
// #include "brake.h"
// #include "signals.h"

// #define LED_DATA_PIN 7
// #define NUM_LEDS 66
// #define LED_TYPE WS2812B
// #define GLOBAL_BRIGHTNESS 255

// // Brightness configuration
// #define MIN_GYRO 0
// #define MAX_GYRO 100

// // Encoder configuration
// #define ENCODER_PIN_A   3
// #define ENCODER_PIN_B   2
// #define ENCODER_CLICK_PIN 8

// int encoderPosition = 0; // Tracks the encoder position
// bool prevA = 1;
// bool prevB = 1;
// bool signal = false;
// bool prevSignal = false;

// CRGB leds[NUM_LEDS];
// Brake brake(leds, NUM_LEDS);
// Signals signals(leds, NUM_LEDS);

// void setup() {
//     FastLED.addLeds<LED_TYPE, LED_DATA_PIN, RGB>(leds, NUM_LEDS);
//     FastLED.clear();
//     FastLED.setBrightness(GLOBAL_BRIGHTNESS);
//     FastLED.show();

//     // Initialize Serial for debugging
//     Serial.begin(115200);
//     Serial.println("Starting...");

//     // Setup encoder pins
//     pinMode(ENCODER_PIN_A, INPUT_PULLUP);
//     pinMode(ENCODER_PIN_B, INPUT_PULLUP);
//     pinMode(ENCODER_CLICK_PIN, INPUT_PULLUP);

//     // Attach interrupts to encoder pins
//     attachInterrupt(digitalPinToInterrupt(ENCODER_PIN_A), handleEncoderA, CHANGE);
//     attachInterrupt(digitalPinToInterrupt(ENCODER_PIN_B), handleEncoderB, CHANGE);
// }

// void loop() {
//     // Prevent encoder values from going beyond the limits
//     if (encoderPosition < -100) {
//         encoderPosition = -100;
//     } else if (encoderPosition > 100) {
//         encoderPosition = 100; // Limit to max value of 100
//     }

//     // Determine if accelerating based on encoder position
//     if (encoderPosition < 0) {
//         brake.accelerating = true;
//         brake.numActiveLEDs = map(-encoderPosition, MIN_GYRO, MAX_GYRO, 0, NUM_LEDS / 2);
//         brake.active_brightness = map(-encoderPosition, MIN_GYRO, MAX_GYRO, brake.minBrakeBrightness, brake.maxBrakeBrightness);
//     } else {
//         brake.accelerating = false;
//         brake.numActiveLEDs = map(encoderPosition, MIN_GYRO, MAX_GYRO, 0, NUM_LEDS / 2);
//         brake.active_brightness = map(encoderPosition, MIN_GYRO, MAX_GYRO, brake.minBrakeBrightness, brake.maxBrakeBrightness);
//     }

//     // Handle flashing if braking is initiated
//     brake.Update();
//     signals.Update();

//     // Debug output
//     // Serial.print("encoderPosition: ");
//     // Serial.print(encoderPosition);
//     // Serial.print(" | Active Brightness: ");
//     // Serial.print(brake.active_brightness);
//     // Serial.print(" | Number of LEDs On: ");
//     // Serial.println(brake.numActiveLEDs * 2);
//     Serial.println();
// }

// // Interrupt Service Routine for ENCODER_PIN_A
// void handleEncoderA() {
//     updateEncoder();
// }

// // Interrupt Service Routine for ENCODER_PIN_B
// void handleEncoderB() {
//     updateEncoder();
// }

// // Function to update encoder position
// void updateEncoder() {
//     signal = digitalRead(ENCODER_CLICK_PIN) == LOW;
//     if (prevSignal == false && signal == true) {
//         Serial.print("trying");
//         signals.left = !signals.left;
//     } 
//     prevSignal = signal;
    
//     bool A = digitalRead(ENCODER_PIN_A);
//     bool B = digitalRead(ENCODER_PIN_B);

//     if (B != prevB) {
//         encoderPosition += (B - prevB) * (A ? +1 : -1);
//     } else if (A != prevA) {
//         encoderPosition += (A - prevA) * (B ? -1 : +1);
//     }

//     prevA = A;
//     prevB = B;
// }
#include <FastLED.h>
#include "brake.h"
#include "signals.h"

#define LED_DATA_PIN 7
#define NUM_LEDS 66
#define LED_TYPE WS2812B
#define GLOBAL_BRIGHTNESS 255

// Brightness configuration
#define MIN_GYRO 0
#define MAX_GYRO 100

// Encoder configuration
#define ENCODER_PIN_A   3
#define ENCODER_PIN_B   2
#define ENCODER_CLICK_PIN 8

int encoderPosition = 0; // Tracks the encoder position
bool prevA = 1;
bool prevB = 1;
bool prevClickState = HIGH;
unsigned long lastDebounceTime = 0;
unsigned long debounceDelay = 50;

CRGB leds[NUM_LEDS];

Signals signals(leds, NUM_LEDS);
Brake brake(&signals, leds, NUM_LEDS);

void setup() {
    FastLED.addLeds<LED_TYPE, LED_DATA_PIN, RGB>(leds, NUM_LEDS);
    FastLED.clear();
    FastLED.setBrightness(GLOBAL_BRIGHTNESS);
    FastLED.show();

    // Initialize Serial for debugging
    Serial.begin(115200);
    while (!Serial) {
        ; // Wait for serial port to connect. Needed for native USB port only
    }
    Serial.println("Starting...");

    // Setup encoder pins
    pinMode(ENCODER_PIN_A, INPUT_PULLUP);
    pinMode(ENCODER_PIN_B, INPUT_PULLUP);
    pinMode(ENCODER_CLICK_PIN, INPUT_PULLUP);

    // Attach interrupts to encoder pins
    attachInterrupt(digitalPinToInterrupt(ENCODER_PIN_A), handleEncoderA, CHANGE);
    attachInterrupt(digitalPinToInterrupt(ENCODER_PIN_B), handleEncoderB, CHANGE);

    Serial.println("Setup complete");
}

void loop() {
    // Prevent encoder values from going beyond the limits
    if (encoderPosition < -100) {
        encoderPosition = -100;
    } else if (encoderPosition > 100) {
        encoderPosition = 100; // Limit to max value of 100
    }

    // Determine if accelerating based on encoder position
    if (encoderPosition < 0) {
        brake.accelerating = true;
        brake.numActiveLEDs = map(-encoderPosition, MIN_GYRO, MAX_GYRO, 0, NUM_LEDS / 2);
        brake.active_brightness = map(-encoderPosition, MIN_GYRO, MAX_GYRO, MIN_BRAKE_BRIGHTNESS, MAX_BRAKE_BRIGHTNESS);
    } else {
        brake.accelerating = false;
        brake.numActiveLEDs = map(encoderPosition, MIN_GYRO, MAX_GYRO, 0, NUM_LEDS / 2);
        brake.active_brightness = map(encoderPosition, MIN_GYRO, MAX_GYRO, MIN_BRAKE_BRIGHTNESS, MAX_BRAKE_BRIGHTNESS);
    }

    // Debounce the encoder click
    bool currentClickState = digitalRead(ENCODER_CLICK_PIN);

    if (currentClickState == LOW && prevClickState == HIGH) {
        signals.left = !signals.left;
    }
    
    prevClickState = currentClickState;
    
    brake.Update();
    signals.Update();
    if (SHOW_MARIO && brake.accelerating && brake.numActiveLEDs > MARIO_STAR_THRESHHOLD) {
        brake.MarioStar();
    }
    else if (brake.flashCount == 0) {
        brake.UpdateBrakeLEDs();
        signals.UpdateSignals();
    }  
    else { // handled outside brake.update() since we want it to go OVER the turn signals
        brake.FlashRedLEDs();
    }
    FastLED.show();

    // Debug output
    // Serial.print("encoderPosition: ");
    // Serial.print(encoderPosition);
    // Serial.print(" | Active Brightness: ");
    // Serial.print(brake.active_brightness);
    // Serial.print(" | Number of LEDs On: ");
    // Serial.println(brake.numActiveLEDs * 2);

    // Serial.println("Loop end");
}

// Interrupt Service Routine for ENCODER_PIN_A
void handleEncoderA() {
    updateEncoder();
}

// Interrupt Service Routine for ENCODER_PIN_B
void handleEncoderB() {
    updateEncoder();
}
// Function to update encoder position
void updateEncoder() {
    bool A = digitalRead(ENCODER_PIN_A);
    bool B = digitalRead(ENCODER_PIN_B);

    if (B != prevB) {
        encoderPosition += (B - prevB) * (A ? +1 : -1);
    } else if (A != prevA) {
        encoderPosition += (A - prevA) * (B ? -1 : +1);
    }

    prevA = A;
    prevB = B;
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