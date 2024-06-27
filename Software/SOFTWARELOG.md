# CHANGELOG:

``Date is in YYYY/MM/DD format.``

**v.A.B.C.D**

**A**: New Release

**B**: Medium Feature

**C**: Minor Feature / Bug Fix

**D**: Patch / Refactoring


## [v.0.3.4.5] - 2024-06-26
**Author:** Antoine
-   Wheel audio fades in and out
    -   now based on speed of the wheel -> speed of playback
-   Added lightswitch audio
-   Changed door audio
-   Made smmoke collisions run on `FixedUpdate`
-   Added some randomness to the smoke force to reduce endless force cycles

**HIGH PRIOTITY:** Refactor project tree, its a mess.

*BUGS*
-   NPC walk sound offset (not on time)
-   `Look rotation viewing vector is zero` "bug", not game breaking
-   `locom_m_basicWalk_30f` too fast for the audio clip. This can be dealt with later
-   Should add a `Collidable` parent class for cleaner code (maybe)
-   Make onPhone independant, static

*NEXT STEPS*
-   Hand tracking/interactions (Meta Quest Pro)
-   Hardware (Arduino + gyroscope)
-   Refine smoke interactions


## [v.0.3.4.4] - 2024-06-25
**Author:** Antoine
-   Npcs are now multithreaded, although they are still slow since they need to make callbacks to update()
-   Added a maximum height to particles before they die
-   Adjusted smoke parameters

**HIGH PRIOTITY:** Refactor project tree, its a mess.

*BUGS*
-   Audio wheels should *FADE* out. Also playback speed based on wheelspeed
-   NPC walk sound offset (not on time)
-   `Look rotation viewing vector is zero` "bug", not game breaking
-   `locom_m_basicWalk_30f` too fast for the audio clip. This can be dealt with later
-   Should add a `Collidable` parent class for cleaner code (maybe)
-   Make onPhone independant, static
-   Find better closing sound

*NEXT STEPS*
-   Hand tracking/interactions (Meta Quest Pro)
-   Hardware (Arduino + gyroscope)
-   Light switches interactions and sound effect
-   Refine smoke interactions


## [v.0.3.4.3] - 2024-06-20
**Author:** Antoine
-   **<ins>SMOKE NOW WORKS</ins>** -> *parameters need to be tweaked for more realism*

**HIGH PRIOTITY:** Refactor project tree, its a mess.

*BUGS*
-   Audio wheels should *FADE* out. Also playback speed based on wheelspeed
-   NPC walk sound offset (not on time)
-   `Look rotation viewing vector is zero` "bug", not game breaking
-   `locom_m_basicWalk_30f` too fast for the audio clip. This can be dealt with later
-   Should add a `Collidable` parent class for cleaner code (maybe)
-   Make onPhone independant, static
-   Find better closing sound

*NEXT STEPS*
-   Make NPCs multithreaded
-   Hand tracking/interactions (Meta Quest Pro)
-   Hardware (Arduino + gyroscope)
-   Light switches interactions and sound effect
-   Refine smoke interactions

## [v.0.2.4.3] - 2024-06-15
**Author:** Antoine
-   Refactored Hierachy although tree is still messy.
-   Tried getting colliders with smoke particles working but still doesnt work
-   Tried applying threads to smoke collisions

**HIGH PRIOTITY:** Refactor project tree, its a mess.

*BUGS*
-   Audio wheels should *FADE* out
-   `Look rotation viewing vector is zero` "bug", not game breaking
-   `locom_m_basicWalk_30f` too fast for the audio clip. This can be dealt with later
-   Should add a `Collidable` parent class for cleaner code (maybe)
-   Make onPhone independant, static
-   Find better closing sound

*NEXT STEPS*
-   Light switches interactions and sound effect


## [v.0.2.4.2] - 2024-06-15
**Author:** Antoine
-   Setup the foundation for particles but theres an issue: particles only collide with objects, not other particles
-   No longer need `lookat` Vectors: `AStarPathfinder.cs` finds it using logic
-   Made it so if two NPCs walk through the same door it doesnt open, close, open close: stop and then replace instead of overlapping
-   Adjusted volumes
-   Trimmed audio lengths
-   Added `Arduino` Inputs and outputs, not tested yet
-   Added kitchens

**HIGH PRIOTITY:** Refactor hierachy and project tree, its a mess.

*BUGS*
-   Audio wheels should *FADE* out
-   `Look rotation viewing vector is zero` "bug", not game breaking
-   `locom_m_basicWalk_30f` too fast for the audio clip. This can be dealt with later
-   Should add a `Collidable` parent class for cleaner code (maybe)
-   Make onPhone independant, static
-   Find better closing sound

*NEXT STEPS*
-   Change furniture
-   Light switches interactions and sound effect
-   Smoke   
-   Paintings


## [v.0.2.3.2] - 2024-06-13
**Author:** Antoine
-   **IMPORTANT:** Interactions are now handles by the object script (must extend interatable)
-   Therefore, door interactions are now handled within the `doors.cs` script
-   NPCs can no longer stack (still possible during race condition I think but good enough)
-   Attached sound source to `low` of door so that it moves witht he door
-   Foot step sounds now work
-   Added Wheel sound effects

*BUGS*
-   `Lookat` Vectors are wrong for NPC positions
-   Audio open should play slightly earlier
-   Audio wheels should play slightly earlier AND should *FADE* out
-   `Look rotation viewing vector is zero` "bug", not game breaking
-   `locom_m_basicWalk_30f` too fast for the audio clip. This can be dealt with later

*NEXT STEPS*
-   Change furniture
-   Light switches interactions and sound effect
-   Smoke   
-   Paintings


## [v.0.2.2.2] - 2024-06-12
**Author:** Antoine
-   Refactored the doors scripts, added audio clips
-   Made pathfinding take into account locked doors (dont go through locked ones)
-   Finalized the reworked scene (still need to add new furniture)
-   Added end game rooms (white light)
-   Added locked doors in each dorm room, including UI and sound effect
-   Refactored NPC locations; now stored in a `JSON` file for better 
memory
-   Fixed bug where audio (doors) could be heard from any distance

*BUGS*
-   **While searching for path, location is set as available, but if it fails to find a path to location: the npcs can stack on one location**
-   Audio open should play slightly earlier
-   Should automatically search for a new path if it failed (not wait another XX seconds)
-   `Look rotation viewing vector is zero` "bug", not game breaking

*Next steps*
-   Change furniture
-   Light switches interactions and sound effect
-   Smoke   
-   Paintings


## [v.0.2.2.1] - 2024-06-10
**Author:** Antoine
-   Finalized pathfinding
-   Doors now move on a "points along path" basis instead of facing the direction
-   Smoothed out NPC rotations
-   Fixed major bug where pathfinding coroutines could stack and mess with each other
-   Fixed bug where animation would change mid path
-   Lots of refactoring in `NPCAI.cs`
-   Removed collisions between NPCs and Player (player cant walk through NPC, but NPC can walk through player)
-   Fixed bug where wouldnt rotate smoothly once arrived at final location
-   Added a default variable to move along path (`bool run=false`)

*BUGS*
-   Main Blender file gaps in between walls/floors...? Very minor, doesnt come up often
-   While searching for path, location is set as available, but if it fails to find a path to location: the npcs can stack on one location
-   Should automatically search for a new path if it failed (not wait another XX seconds)
-   `Look rotation viewing vector is zero` "bug", not game breaking

*Next steps*
-   add for total of 6 rooms
-   change furniture
-   add locked doors to each room
-   End room light thingy
-   Smoke   


## [v.0.2.1.1] - 2024-06-08
**Author:** Antoine
-   **MASSIVE UPDATE**
-   NPC pathfinding now works
-   Opens doors, avoids walls obviously

*BUGS*
-   While searching for path, location is set as available, but if it fails to find a path to location, the npcs can stack
-   Main Blender file gaps in between walls/floors...?
-   Changing walking animations during pathfinding?
-   *Change rotation of NPC **GRADUALLY***


**NOTE BUG: Changes animations twice in NPCAI when time is passed**


## [v.0.1.1.1] - 2024-05-10

**Author:** Antoine

-   Implemented door swinging with keyboard key **"E"**
-   Fixed minor bug where `GetKey` was used instead of `GetKeyDown` (caused rapid changing of door states)
-   Player must be blocked in the **forward direction** by the door for it to work
-   Animations for door working
-   Animator variable are used to store the states of doors

This probably isn't the best method of opening and closing  (should probably use `IsTrigger` colliders)
But seeing as we will be switching to vr shortly, this isn't very important. It is also moving the frame with the door,
Instead of the Frame staying still, and only the door swinging.
<br>
Doors should be able to be pushed closed with a collisions, this can be done later though.


## [v.0.1.0.1] - 2024-05-09

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


## [v.0.0.0.1] - 2024-04-05

**Author:** Antoine

-   Initial Commit