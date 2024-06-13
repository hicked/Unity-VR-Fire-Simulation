using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float baseSpeedMultiplier = 2f;
    [SerializeField] private float sprintSpeedMultiplier = 3.5f;
    [SerializeField] private float maxDistance = 0.2f;
    [SerializeField] private LayerMask layersForCollisions; 
    [SerializeField] private float minHeightBeforeCollisions = 0.1f;  // Note this is the height from the top, so well have to do some arithmetic to convert it into the minimum height from the bottom
    [SerializeField] private float lookingSpeed = 5f;
    private BoxCollider playerHitBox;
    private Vector3 lastBlockedLocation;
    private float horizontalRotation;
    public string lastBlockedByType; // This will be used to check if the layer that is blocking the player is a ramp
    public GameObject lastBlockedByObj;

    private void Start() {
        if (!(transform.localScale.x == transform.localScale.z)) {
            throw new UnityException("Player collider x scale and z scale must be the same");
        }
        playerHitBox = GetComponent<BoxCollider>();
    }

    //KEYBOARD AND MOUSE CONTROLS!!!

    // private void Update() {
    //     Vector3 inputVector = new Vector3(0f, 0f, 0f);
    //     if (Input.GetKey(KeyCode.W) && !isBlocked(transform.forward)) {
    //         inputVector += transform.forward;
    //     }
    //     if (Input.GetKey(KeyCode.A) && !isBlocked(-transform.right)) {
    //         inputVector -= transform.right;
    //     }
    //     if (Input.GetKey(KeyCode.S) && !isBlocked(-transform.forward)) {
    //         inputVector -= transform.forward;
    //     }
    //     if (Input.GetKey(KeyCode.D) && !isBlocked(transform.right)) {
    //         inputVector += transform.right;
    //     }
        
    //     if (inputVector.sqrMagnitude > 1f) { // ensure that the player is the same speed when moving diag
    //         inputVector.Normalize();
    //     }

    //     float speedMultiplier = Input.GetKey(KeyCode.LeftControl) ? sprintSpeedMultiplier : baseSpeedMultiplier;
    //     transform.position += inputVector * Time.deltaTime * speedMultiplier;
    
    //     horizontalRotation = Input.GetAxisRaw("Mouse X");

    //     this.transform.eulerAngles += new Vector3(0, horizontalRotation * lookingSpeed, 0);
    // }

    //Wheel Chair control (for now with arrows and wasd)
    private void Update() {
        float leftWheelSpeed = 0f;
        float rightWheelSpeed = 0f;

        if (Input.GetKey(KeyCode.W) && !isBlocked(transform.forward, maxDistance)) {
            leftWheelSpeed = 1f;
        }
        if (Input.GetKey(KeyCode.S) && !isBlocked(-transform.forward, maxDistance)) {
            leftWheelSpeed = -1f;
        }

        if (Input.GetKey(KeyCode.UpArrow) && !isBlocked(transform.forward, maxDistance)) {
            rightWheelSpeed += 1f;
        }
        if (Input.GetKey(KeyCode.DownArrow) && !isBlocked(-transform.forward, maxDistance)) {
            rightWheelSpeed = -1f;
        }
        
        
        float rotationalSpeed = (leftWheelSpeed - rightWheelSpeed)/playerHitBox.size.x; // positive would be clockwise
        float forwardSpeed = 0f;

        if (leftWheelSpeed > 0 && rightWheelSpeed > 0) {
            forwardSpeed = Mathf.Min(leftWheelSpeed, rightWheelSpeed);
        }
        else if (leftWheelSpeed < 0 && rightWheelSpeed < 0) {
            forwardSpeed = -Mathf.Min(Mathf.Abs(leftWheelSpeed), Mathf.Abs(rightWheelSpeed));
        }

        transform.position += forwardSpeed * transform.forward * Time.deltaTime * baseSpeedMultiplier;
        transform.eulerAngles += new Vector3(0, rotationalSpeed * Time.deltaTime * lookingSpeed, 0);
    }

    public bool isBlocked(Vector3 dir, float distance) {
        RaycastHit hitInfo = default(RaycastHit);
            bool hit = Physics.BoxCast(
                new Vector3(
                    transform.position.x, 
                    transform.position.y - playerHitBox.size.y + (playerHitBox.size.y - minHeightBeforeCollisions)/2f + minHeightBeforeCollisions, 
                    transform.position.z), 
                new Vector3((playerHitBox.size.x - maxDistance)/2f, (playerHitBox.size.y - minHeightBeforeCollisions)/2f, (playerHitBox.size.z - maxDistance)/2f), // IMOPRTANT: set the y to negative, that way its starts from the top down instead of bottom up
                dir, 
                out hitInfo,
                transform.rotation, 
                distance,
                layersForCollisions);
        
        Debug.DrawRay(transform.position, dir * maxDistance, hit ? Color.red : Color.green);
        if (hit) {
            lastBlockedByType = LayerMask.LayerToName(hitInfo.collider.gameObject.layer);
            lastBlockedByObj = hitInfo.collider.gameObject;
            lastBlockedLocation = hitInfo.point;
        }
        return hit;
    }
}

// Note that if you are at an angle against a wall it will not work. Short term this will do but long term we need to incorporate
// Moments into each of the wheel chairs wheel.