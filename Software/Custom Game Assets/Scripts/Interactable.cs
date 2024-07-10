using UnityEngine;

public interface Interactable
{
    void KeyboardInteract();
    void PinchInteract(); // Pinching interaction (index finger and thumb)
    void GrabInteract(GameObject hand); // Grabbing interaction (full hand)
    void PokeInteract(); // Poke interaction (index finger)
}