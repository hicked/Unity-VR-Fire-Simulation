#include <Arduino.h>

#define L_ENC_A 2
#define L_ENC_B 3

#define R_ENC_A 4
#define R_ENC_B 5

#define RESOLUTION 100

volatile int leftCounter = 0;
volatile int rightCounter = 0;

unsigned long lastTime = 0;
unsigned long currentTime = 0;
float leftSpeed = 0;
float rightSpeed = 0;

void setup() {
    Serial.begin(115200);

    pinMode(L_ENC_A, INPUT_PULLUP);
    pinMode(L_ENC_B, INPUT_PULLUP);
    pinMode(R_ENC_A, INPUT_PULLUP);
    pinMode(R_ENC_B, INPUT_PULLUP);

    attachInterrupt(digitalPinToInterrupt(L_ENC_A), readLeftEncoder, CHANGE);
    attachInterrupt(digitalPinToInterrupt(L_ENC_B), readLeftEncoder, CHANGE);
    attachInterrupt(digitalPinToInterrupt(R_ENC_A), readRightEncoder, CHANGE);
    attachInterrupt(digitalPinToInterrupt(R_ENC_B), readRightEncoder, CHANGE);

    lastTime = millis();
}

void loop() {
    currentTime = millis();
    if (currentTime - lastTime >= 100) { // Calculate speed every 100ms
        noInterrupts(); // Disable interrupts while calculating speed
        leftSpeed = (leftCounter / (float)RESOLUTION) / ((currentTime - lastTime) / 1000.0); // Speed in ticks per second
        rightSpeed = (rightCounter / (float)RESOLUTION) / ((currentTime - lastTime) / 1000.0); // Speed in ticks per second
        leftCounter = 0;
        rightCounter = 0;
        lastTime = currentTime;
        interrupts(); // Re-enable interrupts

        // Send data to Unity
        String data = String(leftSpeed) + "," + String(rightSpeed);
        Serial.println(data);
    }

    if (Serial.available()) {
        // READING STUFF FROM UNITY
        String incomingData = Serial.readString();
        // Process incoming data if needed
    }
}

void readLeftEncoder() {
    static uint8_t L_old_AB = 3;  // Lookup table index
    static int8_t L_encval = 0;   // Encoder value
    static const int8_t enc_states[]  = {0,-1,1,0,1,0,0,-1,-1,0,0,1,0,1,-1,0}; // Lookup table

    L_old_AB <<= 2;  // Remember previous state

    if (digitalRead(L_ENC_A)) L_old_AB |= 0x02; // Add current state of pin A
    if (digitalRead(L_ENC_B)) L_old_AB |= 0x01; // Add current state of pin B

    L_encval += enc_states[(L_old_AB & 0x0f)];

    // Update counter if encoder has rotated a full indent, that is at least 4 steps
    if (L_encval > 3) {        // Four steps forward
        leftCounter++;
        L_encval = 0;
    } else if (L_encval < -3) { // Four steps backward
        leftCounter--;
        L_encval = 0;
    }
}

void readRightEncoder() {
    static uint8_t R_old_AB = 3;  // Lookup table index
    static int8_t R_encval = 0;   // Encoder value
    static const int8_t enc_states[]  = {0,-1,1,0,1,0,0,-1,-1,0,0,1,0,1,-1,0}; // Lookup table

    R_old_AB <<= 2;  // Remember previous state

    if (digitalRead(R_ENC_A)) R_old_AB |= 0x02; // Add current state of pin A. Bitwise OR
    if (digitalRead(R_ENC_B)) R_old_AB |= 0x01; // Add current state of pin B

    R_encval += enc_states[(R_old_AB & 0x0f)];

    // Update counter if encoder has rotated a full indent, that is at least 4 steps
    if (R_encval > 3) {        // Four steps forward
        rightCounter++;
        R_encval = 0;
    } else if (R_encval < -3) { // Four steps backward
        rightCounter--;
        R_encval = 0;
    }
}