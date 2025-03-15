#include <Arduino.h>

// #define L_ENC_A 2
// #define L_ENC_B 3

// #define R_ENC_A 4
// #define R_ENC_B 5

// #define RESOLUTION 100

// volatile int leftCounter = 0;
// volatile int rightCounter = 0;

// unsigned long lastTime = 0;
// unsigned long currentTime = 0;
// float leftSpeed = 0;
// float rightSpeed = 0;

// void setup() {
//     Serial.begin(115200);

//     pinMode(L_ENC_A, INPUT_PULLUP);
//     pinMode(L_ENC_B, INPUT_PULLUP);
//     pinMode(R_ENC_A, INPUT_PULLUP);
//     pinMode(R_ENC_B, INPUT_PULLUP);

//     attachInterrupt(digitalPinToInterrupt(L_ENC_A), readLeftEncoder, CHANGE);
//     attachInterrupt(digitalPinToInterrupt(L_ENC_B), readLeftEncoder, CHANGE);
//     attachInterrupt(digitalPinToInterrupt(R_ENC_A), readRightEncoder, CHANGE);
//     attachInterrupt(digitalPinToInterrupt(R_ENC_B), readRightEncoder, CHANGE);

//     lastTime = millis();
// }

// void loop() {
//     currentTime = millis();
//     if (currentTime - lastTime >= 10) { // Calculate speed every 100ms
//         noInterrupts(); // Disable interrupts while calculating speed
//         leftSpeed = (leftCounter / (float)RESOLUTION) / ((currentTime - lastTime) / 1000.0); // Speed in ticks per second
//         rightSpeed = (rightCounter / (float)RESOLUTION) / ((currentTime - lastTime) / 1000.0); // Speed in ticks per second
//         leftCounter = 0;
//         rightCounter = 0;
//         lastTime = currentTime;
//         interrupts(); // Re-enable interrupts

//         // Send data to Unity
//         String data = String(leftSpeed) + "," + String(rightSpeed);
//         Serial.println(data);
//     }

//     if (Serial.available()) {
//         // READING STUFF FROM UNITY
//         String incomingData = Serial.readString();
//         // Process incoming data if needed
//     }
// }

// void readLeftEncoder() {
//     static uint8_t L_old_AB = 3;  // Lookup table index
//     static int8_t L_encval = 0;   // Encoder value
//     static const int8_t enc_states[]  = {0,-1,1,0,1,0,0,-1,-1,0,0,1,0,1,-1,0}; // Lookup table

//     L_old_AB <<= 2;  // Remember previous state

//     if (digitalRead(L_ENC_A)) L_old_AB |= 0x02; // Add current state of pin A
//     if (digitalRead(L_ENC_B)) L_old_AB |= 0x01; // Add current state of pin B

//     L_encval += enc_states[(L_old_AB & 0x0f)];

//     // Update counter if encoder has rotated a full indent, that is at least 4 steps
//     if (L_encval > 3) {        // Four steps forward
//         leftCounter++;
//         L_encval = 0;
//     } else if (L_encval < -3) { // Four steps backward
//         leftCounter--;
//         L_encval = 0;
//     }
// }

// void readRightEncoder() {
//     static uint8_t R_old_AB = 3;  // Lookup table index
//     static int8_t R_encval = 0;   // Encoder value
//     static const int8_t enc_states[]  = {0,-1,1,0,1,0,0,-1,-1,0,0,1,0,1,-1,0}; // Lookup table

//     R_old_AB <<= 2;  // Remember previous state

//     if (digitalRead(R_ENC_A)) R_old_AB |= 0x02; // Add current state of pin A. Bitwise OR
//     if (digitalRead(R_ENC_B)) R_old_AB |= 0x01; // Add current state of pin B

//     R_encval += enc_states[(R_old_AB & 0x0f)];

//     // Update counter if encoder has rotated a full indent, that is at least 4 steps
//     if (R_encval > 3) {        // Four steps forward
//         rightCounter++;
//         R_encval = 0;
//     } else if (R_encval < -3) { // Four steps backward
//         rightCounter--;
//         R_encval = 0;
//     }
// }














#include <Arduino.h>

// Encoder configuration
#define L_ENCODER_PIN_A 5
#define L_ENCODER_PIN_B 4

#define R_ENCODER_PIN_A 2
#define R_ENCODER_PIN_B 3

#define RESOLUTION 4

#define UPDATE_INTERVAL 10

unsigned long lastUpdate = 0;

long rightCounter = 0; // Tracks the encoder position
long leftCounter = 0;

bool LprevA = 1;
bool LprevB = 1;

bool RprevA = 1;
bool RprevB = 1;

void setup() {
    Serial.begin(115200);
    while (!Serial) {
        ; // Wait for serial port to connect. Needed for native USB port only
    }
    Serial.println("Starting...");

    // Setup encoder pins
    pinMode(L_ENCODER_PIN_A, INPUT_PULLUP);
    pinMode(L_ENCODER_PIN_B, INPUT_PULLUP);

    pinMode(R_ENCODER_PIN_A, INPUT_PULLUP);
    pinMode(R_ENCODER_PIN_B, INPUT_PULLUP);

    // Attach interrupts to encoder pins
    attachInterrupt(digitalPinToInterrupt(R_ENCODER_PIN_A), updateRightEncoder, CHANGE);
    attachInterrupt(digitalPinToInterrupt(R_ENCODER_PIN_B), updateRightEncoder, CHANGE);

    // Enable pin change interrupt for left encoder pins
    *digitalPinToPCMSK(L_ENCODER_PIN_A) |= bit(digitalPinToPCMSKbit(L_ENCODER_PIN_A));
    *digitalPinToPCMSK(L_ENCODER_PIN_B) |= bit(digitalPinToPCMSKbit(L_ENCODER_PIN_B));
    PCIFR |= bit(digitalPinToPCICRbit(L_ENCODER_PIN_A));
    PCICR |= bit(digitalPinToPCICRbit(L_ENCODER_PIN_A));

    Serial.println("Setup complete");
}

void loop() {
    // Prevent encoder values from going beyond the limits

    unsigned long currentTime = millis();
    if (currentTime - lastUpdate >= UPDATE_INTERVAL) { // Calculate speed every 100ms
        float leftSpeed = ((float)leftCounter / RESOLUTION) / (currentTime - lastUpdate); // Speed in ticks per second
        float rightSpeed = ((float)rightCounter / RESOLUTION) / (currentTime - lastUpdate); // Speed in ticks per second
        
        // Send data to Unity
        String data = String(leftSpeed) + "," + String(rightSpeed);
        Serial.println(data);

        leftCounter = 0;
        rightCounter = 0;
        lastUpdate = currentTime;
    }

    if (Serial.available()) {
        // READING STUFF FROM UNITY
        String incomingData = Serial.readString();
        // Process incoming data if needed
    }
}

// Function to update left encoder position
ISR(PCINT2_vect) {
    bool LA = digitalRead(L_ENCODER_PIN_A);
    bool LB = digitalRead(L_ENCODER_PIN_B);

    if (LB != LprevB) {
        leftCounter += (LB - LprevB) * (LA ? +1 : -1);
    } else if (LA != LprevA) {
        leftCounter += (LA - LprevA) * (LB ? -1 : +1);
    }

    LprevA = LA;
    LprevB = LB;
}

// Function to update right encoder position
void updateRightEncoder() {
    bool RA = digitalRead(R_ENCODER_PIN_A);
    bool RB = digitalRead(R_ENCODER_PIN_B);

    if (RB != RprevB) {
        rightCounter += (RB - RprevB) * (RA ? +1 : -1);
    } else if (RA != RprevA) {
        rightCounter += (RA - RprevA) * (RB ? -1 : +1);
    }

    RprevA = RA;
    RprevB = RB;
}