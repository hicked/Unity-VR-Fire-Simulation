using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float baseSpeedMultiplier = 2f;
    [SerializeField] private float sprintSpeedMultiplier = 3.5f;
    
    private BoxCollider playerHitBox;
    [SerializeField] private float maxDistance = 0.2f;
    [SerializeField] private LayerMask layersForCollisions; 
    [SerializeField] private float minHeightBeforeCollisions = 0.1f;  // Note this is the height from the top, so well have to do some arithmetic to convert it into the minimum height from the bottom

    private void Start() {
        if (!(transform.localScale.x == transform.localScale.z)) {
            throw new UnityException("Player collider x scale and z scale must be the same");
        }
        playerHitBox = GetComponent<BoxCollider>();
    }
    void Update() {
        Vector3 inputVector = new Vector3(0f, 0f, 0f);
        if (Input.GetKey(KeyCode.W) && canMove(transform.forward)) {
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
        float minHeightBeforeCollisionsFromBottom = playerHitBox.size.y - minHeightBeforeCollisions;
        //Debug.Log(minHeightBeforeCollisionsFromBottom);
        RaycastHit hitInfo;

        bool hit = !Physics.BoxCast(
            new Vector3(transform.position.x, 
                        transform.position.y - playerHitBox.size.y + (playerHitBox.size.y - minHeightBeforeCollisions)/2f + minHeightBeforeCollisions, 
                        transform.position.z), 
            new Vector3((playerHitBox.size.x - maxDistance)/2f, (playerHitBox.size.y - minHeightBeforeCollisions)/2f, (playerHitBox.size.z - maxDistance)/2f), // IMOPRTANT: set the y to negative, that way its starts from the top down instead of bottom up
            dir, 
            out hitInfo,
            transform.rotation, 
            maxDistance,
            layersForCollisions
        );
        Debug.DrawRay(transform.position, dir * maxDistance, hit ? Color.red : Color.green);
        return hit;
    }
}

// Note that if you are at an angle against a wall it will not work. Short term this will do but long term we need to incorporate
// Moments into each of the wheel chairs wheel.