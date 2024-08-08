using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    [SerializeField] public float leftWheelSpeed = 0f;
    [SerializeField] public float rightWheelSpeed = 0f;
    [SerializeField] private float baseSpeedMultiplier = 2f;
    [SerializeField] private float sprintSpeedMultiplier = 3.5f;
    [SerializeField] private float maxDistance = 0.2f;
    [SerializeField] private LayerMask layersForCollisions; 
    [SerializeField] private float minHeightBeforeCollisions = 0.1f;  // Note this is the height from the top, so well have to do some arithmetic to convert it into the minimum height from the bottom
    [SerializeField] private float lookingSpeed = 5f;
    [SerializeField] private float minWheelSpeedBeforeAudio = 0.5f;
    [SerializeField] private float audioFadeDuration = 0.5f;
    private IEnumerator currentAudioCoroutine;
    private bool isWheelAudioPlaying = false;
    public AudioSource wheelAudioSource;
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

    //KEYBOARD AND MOUSE CONTROLS!!! WASD and mouse to look around

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


    //Wheel Chair control WS up down + Arduino
    private void Update() {
        leftWheelSpeed = 0f;
        rightWheelSpeed = 0f;

        // Arduino inputs
        //lock (ArduinoIO.lockObject) {
        if (ArduinoIO.leftSpeed > 0.1f && !isBlocked(transform.forward, maxDistance)) {
            leftWheelSpeed += ArduinoIO.leftSpeed;
        }
        else if (ArduinoIO.leftSpeed < -0.1f && !isBlocked(-transform.forward, maxDistance)) {
            leftWheelSpeed += ArduinoIO.leftSpeed;
        }
        //}

        //lock (ArduinoIO.lockObject) {
        if (ArduinoIO.rightSpeed > 0.1f && !isBlocked(transform.forward, maxDistance)) {
            rightWheelSpeed += ArduinoIO.rightSpeed;
        }
        else if (ArduinoIO.rightSpeed < -0.1f && !isBlocked(-transform.forward, maxDistance)) {
            rightWheelSpeed += ArduinoIO.rightSpeed;
        }
        //}


        // WS up down
        // Left wheel
        if (Input.GetKey(KeyCode.W) && !isBlocked(transform.forward, maxDistance)) {
            leftWheelSpeed = 1f;
        }
        if (Input.GetKey(KeyCode.S) && !isBlocked(-transform.forward, maxDistance)) {
            leftWheelSpeed = -1f;
        }

        // Right wheel
        if (Input.GetKey(KeyCode.UpArrow) && !isBlocked(transform.forward, maxDistance)) {
            rightWheelSpeed += 1f;
        }
        if (Input.GetKey(KeyCode.DownArrow) && !isBlocked(-transform.forward, maxDistance)) {
            rightWheelSpeed = -1f;
        }
        
        // Angular velocity if wheel speeds are different directions + & -
        float rotationalSpeed = (leftWheelSpeed - rightWheelSpeed)/playerHitBox.size.x; // positive would be clockwise
        float forwardSpeed = 0f;

        // Tangential velocity
        if (leftWheelSpeed > 0 && rightWheelSpeed > 0) {
            forwardSpeed = Mathf.Min(leftWheelSpeed, rightWheelSpeed);
        }
        else if (leftWheelSpeed < 0 && rightWheelSpeed < 0) {
            forwardSpeed = -Mathf.Min(Mathf.Abs(leftWheelSpeed), Mathf.Abs(rightWheelSpeed));
        }


        // Audio stuff
        if ((leftWheelSpeed != 0 || rightWheelSpeed != 0) && !isWheelAudioPlaying) {
            if (currentAudioCoroutine != null) {
                StopCoroutine(currentAudioCoroutine);
            }
            currentAudioCoroutine = FadeIn(wheelAudioSource, audioFadeDuration);
            StartCoroutine(currentAudioCoroutine);
            isWheelAudioPlaying = true;
        }
        else if (Mathf.Max(Mathf.Abs(leftWheelSpeed), Mathf.Abs(rightWheelSpeed)) < minWheelSpeedBeforeAudio && isWheelAudioPlaying) {
            if (currentAudioCoroutine != null) {
                StopCoroutine(currentAudioCoroutine);
            }
            currentAudioCoroutine = FadeOut(wheelAudioSource, audioFadeDuration);
            StartCoroutine(currentAudioCoroutine);
            isWheelAudioPlaying = false;
        }
        

        // SPEED OF THE WHEEL IMPACTS SPEED THE AUDIO CLIP PLAYS AT
        if (isWheelAudioPlaying) {
            wheelAudioSource.pitch = Mathf.Max(Mathf.Abs(leftWheelSpeed), Mathf.Abs(rightWheelSpeed)) * 1;
            // currently it is times one as this is with keyboard input, but will need to be adjusted for arduino input
            //
            // 
            //
            //
            //
        }
        // Apply the movement
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

    private IEnumerator FadeIn(AudioSource audioSource, float duration) {
        audioSource.Play();
        float startVolume = audioSource.volume;
        float targetVolume = 1f; // Maximum volume
        float time = 0;
        // Set audio to start from a random point
        audioSource.time = Random.Range(0f, audioSource.clip.length);

        while (time < duration) {
            audioSource.volume = Mathf.Lerp(startVolume, targetVolume, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        audioSource.volume = targetVolume;
    }

    private IEnumerator FadeOut(AudioSource audioSource, float duration) {
        float startVolume = audioSource.volume;
        float targetVolume = 0f; // Minimum volume
        float time = 0;

        while (time < duration) {
            audioSource.volume = Mathf.Lerp(startVolume, targetVolume, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        audioSource.volume = targetVolume;
        audioSource.Stop();
    }
}