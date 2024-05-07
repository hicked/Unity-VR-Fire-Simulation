using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardControls : MonoBehaviour
{
    [SerializeField] private float baseSpeedMultiplier = 2f;
    [SerializeField] private float sprintMultiplier = 3.5f;

    private BoxCollider playerCollider;

    void Start() {
        playerCollider = GetComponent<BoxCollider>(); // use physics rays to measure distances instead of depending on game engine physics
    }
    void Update() {
        Vector3 inputVector = new Vector3(0f, 0f, 0f);
        if (Input.GetKey(KeyCode.W)) {
            inputVector.z += 1f;
        }
        if (Input.GetKey(KeyCode.A)) {
            inputVector.x -= 1f;
        }
        if (Input.GetKey(KeyCode.S)) {
            inputVector.z -= 1f;
        }
        if (Input.GetKey(KeyCode.D)) {
            inputVector.x += 1f;
        }
        
        if (inputVector.sqrMagnitude > 1f) { // ensure that the player is the same speed when moving diag
            inputVector.x /= inputVector.sqrMagnitude;
            inputVector.y /= inputVector.sqrMagnitude;
            inputVector.z /= inputVector.sqrMagnitude;
        }

        transform.position += (inputVector * Time.deltaTime) * (!(Input.GetKey(KeyCode.LeftControl)) ? baseSpeedMultiplier : sprintMultiplier);
    }
}
