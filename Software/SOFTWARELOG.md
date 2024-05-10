# CHANGELOG:

``Date is in YYYY/MM/DD format.``

**v.A.B.C.D**

**A**: Major Feature / Major Bug Fix

**B**: Minor Feature / Minor Bug Fix

**C**: Refactoring / Tiny Feature

**D**: Patch / Very Minor Thing

## [v.0.1.0.0] - 2024-05-09

**Author:** Antoine

-   Implemented door swinging with keyboard key **"E"**
-   Player must be blocked in the **forward direction** by the door for it to work
-   No animation as of yet
-   Doors must start with either the tag `Closed` or `Open`, and then will switch the tag on interaction

This probably isn't the best method of opening and closing  (should probably use `IsTrigger` colliders)
But seeing as we will be switching to vr shortly, this isn't very important.


## [v.0.1.0.0] - 2024-05-09

**Author:** Antoine

-   Completed player physics and **Keyboard** controls.
-   No turning of the head through mouse mouvement yet (not sure if I will implement it)
-   Replaced `RayCasting` with `BoxCast` for more realistic collisions
-   Will collide with specified layers, for now `Walls, Floor, Doors and Barriers`. Can be changed in the Serialized parameters.
-   Sprinting through `CTRL`
-   Implemented the *last layer to block the player* in order to have special interactions on contact with these layers (**CONTACT** not **COLLISION**. **Rays** not **Colliders**)
-   Implemented minimum height for contacts, otherwise player will walk over it as intended (carpet, rug, ball, etc.)
-   Created/imported lots of *prefabs*. 

*NOTE: This will have to be reworked in order do work with two wheels and thus will have to make the movement work like its on a hinge. Core script should remain similar*


## [v.0.0.1.0] - 2024-04-05

**Author:** Antoine

-   Initial Commit