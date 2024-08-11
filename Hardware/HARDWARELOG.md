# CHANGELOG:

Date is in YYYY/MM/DD format.

**v.A.B.C**

**A**: Major Feature

**B**: Medium Feature

**C**: Minor Feature / Bug Fix

**D**: Patch / Refactoring


## [v.0.1.1.2-prealpha] - 2024-08-08

**Author:** Antoine
**Components**:
-   Rotary Encoders should be considered for mesuring speeds instead of reading voltages
-   *2* DC Brushless Motors OR GEARMOTOR (PG71 & PG188 RS775-5)
    -   100-150W
-   *2* Power Supplies
    -   12V 5A power should do
-   H-Bridge Motor Driver or ESC (Sabertooth dual 25A motor driver)
    -   For controlling the speed and direction

**Specifications**:
-   Wheel Diameter: 24" or 0.61m
-   Top Speed: 10km/h or 2.78m/s
-   Weight: 180lbs for the person and 50lbs for the wheelchair
>   This will mean that the circumference is 1.92m and at 10km/h this means it must have a frequency of 1.45hz or 87RPM. Assuming a weight of 104.3kg (22.7 + 81.6), a radius of 0.305m, and a coefficient of 0.015, Fg when free falling would be of 1022.2N, making friction 15.33N, and a torque of **4.68Nm**

Therefore, we need a motor that can supply around 4.6Nm of torque, with a max rpm of 87. Note these are overkill.
Assuming we have a 12V 5A power supply, we will need a 0.92kt(Nm/A) and 7.25kv(RPM/V)



## [v.0.1.1.1-prealpha] - 2024-08-08

**Author:** Antoine
-   Got outgoing messages and serial port communication up and running
-   Changes baud rate and `delay()`


## [v.0.0.1.1-prealpha] - 2024-08-03

**Author:** Antoine
**Plan:**
-   Use IR sensor to mesure the RPM or m/s of the wheel
-   Use a motor to read the current caused by the rotation of the wheel
-   Map the current -> wheel speed
-   IR is therefore just there to calibrate it
-   *Send* the wheel speed to Unity to map VR movement
-   *"Artificial ground"* for simulated friction, could also use the motors
-   Thrust bearing for rotation of wheel chair

## [v.0.0.0.1-prealpha] - 2024-04-05

**Author:** Antoine

-   Initial Commit