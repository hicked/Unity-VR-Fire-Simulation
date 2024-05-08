using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float baseSpeedMultiplier = 2f;
    [SerializeField] private float sprintMultiplier = 3.5f;
    [SerializeField] private float playerCollisionRadius = 1f;
    [SerializeField] private float maxDistance = 0.2f;


    private void start() {
        if (!(transform.localScale.x == transform.localScale.z)) {
            throw new UnityException("Player collider must be cylindrical and therefore x scale and z scale must be the same");
        }
    }
    void Update() {
        Vector3 inputVector = new Vector3(0f, 0f, 0f);
        Debug.Log(canMove(Vector3.forward));
        if (Input.GetKey(KeyCode.W) && canMove(Vector3.forward)) {
            Debug.Log("pressing w and can move");
            inputVector += Vector3.forward;
        }
        if (Input.GetKey(KeyCode.A) && canMove(Vector3.left)) {
            inputVector += Vector3.left;
        }
        if (Input.GetKey(KeyCode.S) && canMove(Vector3.back)) {
            inputVector += Vector3.back;
        }
        if (Input.GetKey(KeyCode.D) && canMove(Vector3.right)) {
            inputVector += Vector3.right;
        }
        
        if (inputVector.sqrMagnitude > 1f) { // ensure that the player is the same speed when moving diag
            inputVector.x /= inputVector.sqrMagnitude;
            inputVector.y /= inputVector.sqrMagnitude;
            inputVector.z /= inputVector.sqrMagnitude;
        }

        transform.position += (inputVector * Time.deltaTime) * (!(Input.GetKey(KeyCode.LeftControl)) ? baseSpeedMultiplier : sprintMultiplier);
    }
    
    public bool canMove(Vector3 dir) {
        return !(Physics.CapsuleCast((transform.position + new Vector3(0f, -0.1f, 0f)), 
                (transform.position + new Vector3(0f, 0.9f, 0f)), 
                playerCollisionRadius, 
                dir, 
                maxDistance));

        //return !(Physics.Raycast(transform.position, dirVector, playerCollisionRadius));
    }
}