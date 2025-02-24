# CHANGELOG:

``Date is in YYYY/MM/DD format.``

**v.A.B.C.D**

**A**: New Release

**B**: Major Feature

**C**: Minor Feature / Bug Fix

**D**: Patch / Refactoring

## [v.0.7.1.0-beta] - 2025-02-214
**Author:** Antoine
*Changes*:
-   NPCs queue now works and they properly run out when fire is detected or the alarm sounds
-   Barrier is now setup and works properly
-   Room door opens when fire starts just so player can see the fire and smoke can come out
-   Added props to lobby. Will have to add demo instructions
-   Added new stairway door asset
-   Added main lobby this will contain a few items in the future to demo the controls and whatnot
-   Added `SceneLoader` script for collider that switched scenes

*Current Bugs*:
-   **Work on making pathfinding to exit more fluid. They should have more urgency since they are leaving the building due to a fire
-   PROJECT TREE CLEANUP STILL NEEDS TO BE COMPLETED: Fire, NPCs, Doors, Lights
-   `Look rotation viewing vector is zero` "bug", not game breaking
-   Add different walk and run sounds BROKEN
-   Fix stairway door prefab asset folder with copies
-   enqueue points when pathfinding

*Next Steps*:
-   Sequence of events:
    -   Fire starts (make sure no player in the room)
        -   Imaginary wall
        -   Npcs with LOS run out towards exits
        -   Alarm rings (15 secs after fire ini)
            -   all other NPCs run out
        -   close and locks door to fire
        -   time before death TBD
-   Last room (3)
-   Resize objects for more realism (props mostly)
-   UI/Menu -> only at the end
-   Make wheelaudio change based on wheel speeds (should be simple)
-   Make onPhone independant, static
-   Fix Walking sound effect not working
-   Add demo instructions
-   Add end screen


## [v.0.7.0.0-beta] - 2025-02-214
**Author:** Antoine
*Changes*:
-   NPCs now pathfind to exit when they either hear the alarm or see the fire
-   The alarm sounds shortly after the fire starts
-   Barrier is created preventing player from going into the room with the fire once all npcs/players have left the room  
-   NPCs now are destroyed when they get to one of the exits

- Fixed Close door sound being broken
- Fixed alarm not sounding at the right time
- Fixed Red doors not being keyboard or VR interactable
- Fixed lookat rotation not rotating NPC when arrived at location
- Fixed bugs with A* queue

*Current Bugs*:
-   **Work on making pathfinding to exit more fluid. They should have more urgency since they are leaving the building due to a fire. Need to look into the queuing systems**
-   PROJECT TREE CLEANUP STILL NEEDS TO BE COMPLETED: Fire, NPCs, Doors, Lights
-   `Look rotation viewing vector is zero` "bug", not game breaking
-   Add different walk and run sounds
-   Smoke ignoring colliders at X time

*Next Steps*:
-   Sequence of events:
    -   Fire starts (make sure no player in the room)
        -   Imaginary wall
        -   Npcs with LOS run out towards exits
        -   Alarm rings (15 secs after fire ini)
            -   all other NPCs run out
        -   close and locks door to fire
        -   time before death TBD
-   Last room (3)
-   Resize objects for more realism (props mostly)
-   UI/Menu -> only at the end
-   Make wheelaudio change based on wheel speeds (should be simple)
-   Make onPhone independant, static


## [v.0.6.7.9-beta] - 2024-11-25
**Author:** Antoine
*Changes*:
-   Added a door swing multiplier since doors were not swinging the appropriate amount

*Current Bugs*:
-   Close door sound now broken
-   PROJECT TREE CLEANUP STILL NEEDS TO BE COMPLETED: Fire, NPCs, Doors, Lights
-   `Look rotation viewing vector is zero` "bug", not game breaking
-   Add different walk and run sounds
-   Smoke ignoring colliders at X time
-   Alarm not sounding at the right time

*Next Steps*:
-   Sequence of events:
    -   Fire starts (make sure no player in the room)
        -   Imaginary wall
        -   Npcs with LOS run out towards exits
        -   Alarm rings (15 secs after fire ini)
            -   all other NPCs run out
        -   close and locks door to fire
        -   time before death TBD
-   Last room (3)
-   Resize objects for more realism (props mostly)
-   UI/Menu -> only at the end
-   Make wheelaudio change based on wheel speeds (should be simple)
-   Make onPhone independant, static


## [v.0.6.7.8-beta] - 2024-08-19
**Author:** Antoine
*Changes*:
-   Doors *swings* fully working (with projections)
-   Added wine cabinet prefab back
-   Removed duplicate prefabs and moved files around
-   Fixed Closing jittering (with AND without NPCs)
-   Alarm working-ish
-   Basics of fire management (spawning, smoke ignores doors etc)

*Current Bugs*:
-   Close door sound now broken
-   PROJECT TREE CLEANUP STILL NEEDS TO BE COMPLETED: Fire, NPCs, Doors, Lights
-   `Look rotation viewing vector is zero` "bug", not game breaking
-   Add different walk and run sounds
-   Smoke ignoring colliders at X time
-   Alarm not sounding at the right time

*Next Steps*:
-   Sequence of events:
    -   Fire starts (make sure no player in the room)
        -   Imaginary wall
        -   Npcs with LOS run out towards exits
        -   Alarm rings (15 secs after fire ini)
            -   all other NPCs run out
        -   close and locks door to fire
        -   time before death TBD
-   Last room (3)
-   Resize objects for more realism (props mostly)
-   UI/Menu -> only at the end
-   Make wheelaudio change based on wheel speeds (should be simple)
-   Make onPhone independant, static


## [v.0.6.6.8-beta] - 2024-08-08
**Author:** Antoine
*Changes*:
-   **Arduino + Unity connection now works**
-   Candle flames now properly follow stick
-   Reworked candle flame extinguishing
> ***NOTE: `locking` wheelspeeds for threadsafety messes with input delay, so ive disabled it for now unless we start getting race conditions***

*Current Bugs*:
-   Missing `Wine Cabinet` prefab
-   PROJECT TREE CLEANUP STILL NEEDS TO BE COMPLETED: Fire, NPCs, Doors, Lights AS WELL AS DUPLICATES
-   `Look rotation viewing vector is zero` "bug", not game breaking
-   Add different walk and run sounds
-   Smoke ignoring colliders at X time

*Next Steps*:
-   Last room (3)
-   Make doors keep momentum slightly (swing)
-   Resize objects for more realism (props mostly)
-   ALARM
-   UI/Menu -> only at the end
-   Make wheelaudio change based on wheel speeds (should be simple)
-   Make onPhone independant, static


## [v.0.5.6.8-beta] - 2024-08-07
**Author:** Antoine
-   Assigned lights to switches.
-   Changed candle prefab to be seperate `XR interactable` (doesnt change anything functionnaly, just resolves a warning). *Note this is broken since the candle lights dont track with the candlestick.
-   Communication Unity -> Arduino works but port access is denied for whatever reason

*BUGS*:
-   **PROJECT TREE CLEANUP STILL NEEDS TO BE COMPLETED**: Fire, NPCs, Doors, Lights AS WELL AS DUPLICATES
-   `Look rotation viewing vector is zero` "bug", not game breaking
-   Make onPhone independant, static
-   Add different walk and run sounds
-   Smoke ignoring colliders at X time

*NEXT STEPS*
-   Last room (3)
-   Make doors keep momentum slightly (swing)
-   Resize objects for more realism (props mostly)
-   ALARM
-   UI/Menu -> only at the end


## [v.0.5.5.8-beta] - 2024-07-25
**Author:** Antoine
**First beta version: Most Core Elements Completed**
*NOTE*: There is a limit of 8 lights per-camera per-object. Might be worth looking at `HDRP`, or simply baking the lights in instead or something.

*BUGS*:
-   DOULBE PREFAB FOR COMPUTER TABLE AND BEIGE CHAIR SMALL COFFEE TABLE TV STUFF
-   **PROJECT TREE CLEANUP STILL NEEDS TO BE COMPLETED**: Fire, NPCs, Doors, Lights
-   Some switches dont have assigned lights
-   `Look rotation viewing vector is zero` "bug", not game breaking
-   `locom_m_basicWalk_30f` too fast for the audio clip. This can be dealt with later
-   Make onPhone independant, static
-   Add different walk and run sounds
-   Smoke ignoring colliders at X time

*NEXT STEPS*
-   Make doors keep momentum slightly (swing)
-   Resize objects for more realism (props mostly)
-   ALARM
-   Hardware (Arduino + gyroscope)
-   UI/Menu -> only at the end

## [v.0.4.5.8-alpha] - 2024-07-18
**Author:** Antoine
-   Stopped doors from swinging the wrong way
-   Added damper to doors
-   Candles now properly go out on pinch
-   Light switches now work

*BUGS*:
-   **IMPORTANT** Doors need to change their min/max based on their orientation to begin

-   **PROJECT TREE CLEANUP STILL NEEDS TO BE COMPLETED**: Fire, NPCs, Doors, Lights
-   `Look rotation viewing vector is zero` "bug", not game breaking
-   `locom_m_basicWalk_30f` too fast for the audio clip. This can be dealt with later
-   Make onPhone independant, static
-   Add different walk and run sounds
-   Smoke ignoring colliders are X time

*NEXT STEPS*
-   **POKE INTERACTIONS** light switch
-   Add flame going out sound
-   Resize objects for more realism (props mostly)
-   ALARM
-   Hardware (Arduino + gyroscope)
-   UI/Menu


## [v.0.3.5.8-alpha] - 2024-07-18
**Author:** Antoine
-   Doors now completely work (jitter sometimes? rb problem)
-   Mirrors work-ish need to prerender them. Might be good enough. Consult

*BUGS*:
-   **PROJECT TREE CLEANUP STILL NEEDS TO BE COMPLETED**: Fire, NPCs, Doors, Lights
-   `Look rotation viewing vector is zero` "bug", not game breaking
-   `locom_m_basicWalk_30f` too fast for the audio clip. This can be dealt with later
-   Make onPhone independant, static
-   Add different walk and run sounds
-   Smoke ignoring colliders are X time

*NEXT STEPS*
-   **POKE INTERACTIONS** light switch
-   Add flame going out sound
-   Resize objects for more realism (props mostly)
-   ALARM
-   Hardware (Arduino + gyroscope)
-   UI/Menu

## [v.0.3.5.7-alpha] - 2024-07-16
**Author:** Antoine
-   Massive refactor of project tree and files
-   Doors now work and swing properly

*BUGS*
-   Cant interact with doors vr bug
-   doors a little bit jittery when standing still
-   **PROJECT TREE CLEANUP STILL NEEDS TO BE COMPLETED**
-   `Look rotation viewing vector is zero` "bug", not game breaking
-   `locom_m_basicWalk_30f` too fast for the audio clip. This can be dealt with later
-   Make onPhone independant, static
-   Add different walk and run sounds
-   Smoke ignoring colliders are X time

*NEXT STEPS*
-   Custom interactions -> `override` function. Need to read XR Docs
-   Hardware (Arduino + gyroscope)


## [v.0.3.5.6-prealpha] - 2024-07-10
**Author:** Antoine
-   Interactions now work.
-   Doors might work? Not tested in VR

**HIGH PRIOTITY:** Refactor project tree, its a mess.

*BUGS*
-   `Look rotation viewing vector is zero` "bug", not game breaking
-   `locom_m_basicWalk_30f` too fast for the audio clip. This can be dealt with later
-   Make onPhone independant, static
-   Add different walk and run sounds
-   Smoke ignoring colliders are X time

*NEXT STEPS*
-   Custom interactions -> `override` function. Need to read XR Docs
-   Hardware (Arduino + gyroscope)


## [v.0.3.5.5-prealpha] - 2024-06-27
**Author:** Antoine
-   Hand tracking now works, but not interactions
-   Audio is cut more when it goes through a wall

**HIGH PRIOTITY:** Refactor project tree, its a mess.

*BUGS*
-   `Look rotation viewing vector is zero` "bug", not game breaking
-   `locom_m_basicWalk_30f` too fast for the audio clip. This can be dealt with later
-   Should add a `Collidable` parent class for cleaner code (maybe)
-   Make onPhone independant, static
-   Add different walk and run sounds
-   Smoke ignoring colliders are X time

*NEXT STEPS*
-   Hand tracking/interactions (Meta Quest Pro)
-   Hardware (Arduino + gyroscope)


## [v.0.3.4.5-prealpha] - 2024-06-27
**Author:** Antoine
-   Added flames and explosion with sound
-   Changed Smoke texture. Also changes over time
-   Smoke collisions give downwards velocity automatically
>   *MAKE SURE LIGHTS ARE ARENT REFLECTING PROPERLY ARE SET TO HIGH IMPORTANCE*

**HIGH PRIOTITY:** Refactor project tree, its a mess.

*BUGS*
-   `Look rotation viewing vector is zero` "bug", not game breaking
-   `locom_m_basicWalk_30f` too fast for the audio clip. This can be dealt with later
-   Should add a `Collidable` parent class for cleaner code (maybe)
-   Make onPhone independant, static
-   Add different walk and run sounds
-   Smoke ignoring colliders are X time

*NEXT STEPS*
-   Hand tracking/interactions (Meta Quest Pro)
-   Hardware (Arduino + gyroscope)
-   Refine smoke interactions
-   Audio needs collisions with walls (get muffled if goes through walls)


## [v.0.3.4.5-prealpha] - 2024-06-26
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
-   Add flame to smoke


## [v.0.3.4.4-prealpha] - 2024-06-25
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


## [v.0.3.4.3-prealpha] - 2024-06-20
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

## [v.0.2.4.3-prealpha] - 2024-06-15
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


## [v.0.2.4.2-prealpha] - 2024-06-15
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


## [v.0.2.3.2-prealpha] - 2024-06-13
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


## [v.0.2.2.2-prealpha] - 2024-06-12
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


## [v.0.2.2.1-prealpha] - 2024-06-10
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


## [v.0.2.1.1-prealpha] - 2024-06-08
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


## [v.0.1.1.1-prealpha] - 2024-05-10

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


## [v.0.1.0.1-prealpha] - 2024-05-09

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


## [v.0.0.0.1-prealpha] - 2024-04-05

**Author:** Antoine

-   Initial Commit