using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float baseSpeedMultiplier = 2f;
    [SerializeField] private float sprintSpeedMultiplier = 3.5f;
    [SerializeField] private float playerCollisionRadius = 0.2f;
    [SerializeField] private float playerHeight = 2f;
    [SerializeField] private float maxDistance = 0.2f;
    [SerializeField] private LayerMask layerForCollisions; 


    private void start() {
        if (!(transform.localScale.x == transform.localScale.z)) {
            throw new UnityException("Player collider must be cylindrical and therefore x scale and z scale must be the same");
        }
    }
    void Update() {
        Vector3 inputVector = new Vector3(0f, 0f, 0f);
        Debug.Log(canMove(Vector3.forward));
        if (Input.GetKey(KeyCode.W) && canMove(transform.forward)) {
            Debug.Log("pressing w and can move");
            inputVector += transform.forward;
        }
        if (Input.GetKey(KeyCode.A) && canMove(-transform.right)) {
            inputVector -= transform.right;
        }
        if (Input.GetKey(KeyCode.S) && canMove(-transform.forward)) {
            inputVector -= transform.forward;
        }
        if (Input.GetKey(KeyCode.D) && canMove(transform.right)) {
            inputVector += transform.right;
        }
        
        if (inputVector.sqrMagnitude > 1f) { // ensure that the player is the same speed when moving diag
            inputVector.Normalize();
        }

        float speedMultiplier = (Input.GetKey(KeyCode.LeftControl)) ? sprintSpeedMultiplier : baseSpeedMultiplier;
        transform.position += inputVector * Time.deltaTime * speedMultiplier;
    }
    
    public bool canMove(Vector3 dir) {
        return !Physics.CapsuleCast(
            transform.position, 
            transform.position + Vector3.up * playerHeight, 
            playerCollisionRadius, 
            dir, 
            maxDistance, 
            layerForCollisions
        );

    }
}

// Note that if you are at an angle against a wall it will not work. Short term this will do but long term we need to incorporate
// Moments into each of the wheel chairs wheel.