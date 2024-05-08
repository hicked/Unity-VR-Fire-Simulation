- A scene is often a level, a barrier where scripts stay constant.
Will probably only have one scene in that case
- Project section contains all assets/ressources you can use in the game. Importing is always an option
- Hierarchy contains `game objects` these can be anything, they are containers with a position, rotation and scale. Sub game objects can are components, like the objects physics, the way it acts, colour etc. Sub-objects ALWAYS have the same position as their parent, so if youre doing something like flappy bird, you should have a game object bird, and then two subobject pngs, one for the bird, one for the wing, where if you move BIRD, both objects will move. You want the objects to be seperate since the wing will move (if you flap)
- Each subcomponent (appears in inspector) cannot talk to each other. You need to set up a public reference w/ `public "OBJTYPE" "OBJNAME"` so like if you have a physics game object, and you want to CHANGE its physics with a key input, youd have to do `public rigidBodyPhysics wheelChair;` and then `wheelChair.velocity += 10`. Obv this will be more difficult with vectors. Any public variable/object will be able to be changed in the unity inspector. This CAN be an issue since anywhwere in the code you can edit this property, so good practice is to leave the variable private, but sure the prefix `[SerializeField]` to make it public to ONLY the game object.
<br><br>
-   If you want to update something based on time, and not framerate (default loop) use `* Time.deltaTime` to scale it.
Will need this for the smoke physics since we want the smoke to move at the same speed, and not go faster if the framerate is higher -> since the loop executes more times.
<br><br>

-   `Prefab`: Prefabricated Game object. An object that takes in parameters
-   `Instantiate`: Spawn of initialize prefab/gameobject. You can have prefabs that are instantiated later, or at a frequency with a script. Will need this for a smoke spawner.
`https://docs.unity3d.com/560/Documentation/Manual/EventFunctions.html` 
<br>Other commands

-   `Destroy()`: Used to delete game object. Don't want to leak memory, or have infinite smoke generating, which will eventually lag.
-   **`Debug.log()`** **IMPORTANT**: Like a `print` or `console.log()` should be used for debugging.
<br><br>

-   If you want to call a function from a script, from a game object, but the object is not static (aka spawns, and gets deleted), you cant hardcore saying which one you would like to use, since the source changes consistently, so you have to find the script from the object each time it runs. TLDR: This is if you need to find a component of a game object **DURING** runtime:<br>
First initialize the script object: `public LogicScript logic;`<br>
Then let `logic = GameObject.FindGameObjectWithTag("logicScript").getComponent<LogicScript>();` This entire line RETURNS a **game object**. <br>
Note `LogicScript` is the name of the `.cs` and you have to give your *game object* a *Tag* named `LogicScript`.<br>
**IT FINDS THE *FIRST* OBJECT WITH THE TAG**
<br><br>

-   Game Objects can have multiple layer, and you can see for which layer it collides. This will be important for the wheelchair as there can be a side layer, front later etc to see where the colision is, and thus determine which `velocity = 0`.
<br><br>

-   `using UnityEngine.UI;` to import UI objects if you want to make a reference to one. Same goes for `UnityEngine.SceneManagement`.

-   Always make visual components children, and add scripts to the parent

-   We can use `Physics material` assets to change the bounciness, dynamic/static friction for the wheels of the wheelchair under the rigid body parameters.

-   `isTrigger` makes it so there is no physical collision, only a trigger created when it gets hit (ghost collider)

-   `isKinematic`: Don't let it move (immovable object)

-   `Movement types:`<br>`Instantaneous`: Goes through walls, instant. <br>`Kinematic`: Goes through walls, delayed.<br>`Velocity tracking`: Keeps rigid body, delayed.

-   Use `Poke Interactor` specifically for buttons/sliders.

-   `Alpha Clipping` is used when we want to make an object partially transparent. We set the texture/material to opaque, and then any pixel from the texture with an alpha smaller than the threshold provided will not be rendered.
