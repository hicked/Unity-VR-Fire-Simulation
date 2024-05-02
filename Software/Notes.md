- A scene is often a level, a barrier where scripts stay constant.
Will probably only have one scene in that case
- Project section contains all assets/ressources you can use in the game. Importing is always an option
- Hierarchy contains `game objects` these can be anything, they are containers with a position, rotation and scale. Sub game objects can are components, like the objects physics, the way it acts, colour etc
- Each subcomponent (appears in inspector) cannot talk to each other. You need to set up a public reference w/ `public "OBJTYPE" "OBJNAME"` so like if you have a physics game object, and you want to CHANGE its physics with a key input, youd have to do `public rigidBodyPhysics wheelChair;` and then `wheelChair.velocity += 10`. Obv this will be more difficult with vectors. Any public variable/object will be able to be changed in the unity inspector.

