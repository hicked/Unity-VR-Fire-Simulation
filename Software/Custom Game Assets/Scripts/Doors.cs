using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Doors : Audible, Interactable {
    [SerializeField] public AudioSource doorAudioSource;
    [SerializeField] public AudioClip openDoorClip;
    [SerializeField] public AudioClip closeDoorClip;

    [SerializeField] public float closeOffset = 0.55f;
    [SerializeField] public float openOffset = 0.55f;
    [SerializeField] private float doorSpeed = 90f;
    [SerializeField] public float doorswingMultiplier = 3.0f;

    [SerializeField] public float openAngle = 90f;
    [SerializeField] public float closedAngle = 0f;
    [SerializeField] private float angleSnap = 5f; // door will snap closed if the angle is less than this
    [SerializeField] public GameObject doorHandle;
    private DoorHandle handleScript;
    [SerializeField] private HingeJoint doorHinge;
    private Rigidbody doorRigidBody; // MIGHT NOT NEED THIS TO RESET VELOCITY OF DOOR MIGHT NEED HINGEJOINT VELOCITY
    private Vector3 closedVector;

    // used for npcs opening doors, not player
    private float targetRotation;
    public float angle;
    public bool movedByNPC = false;
    public bool swinging = false;
    [SerializeField] public float swingMultiplier = 0.5f;
    
    private Coroutine temporaryOpenCoroutine;

    public void KeyboardInteract() {
        handleScript = doorHandle.GetComponent<DoorHandle>();
        if (handleScript.isLocked) {
            if (!(handleScript.isMessageDisplaying)) {
                handleScript.doorAudioSource.clip = handleScript.lockedDoorClip;
                handleScript.doorAudioSource.Play();
                StartCoroutine(handleScript.ShowLockedDoorMessage(handleScript.lockedDoorMessageFadeIn, handleScript.lockedDoorMessageTime, handleScript.lockedDoorMessageFadeOut));
            }
        }
        else {
            if (angle == openAngle) {
                Close();
            }
            else {
                Open();
            }
        }
    }

    // Start is called before the first frame update
    void Start() {
        closedVector = transform.parent.forward;
        doorRigidBody = GetComponent<Rigidbody>();
        handleScript = doorHandle.GetComponent<DoorHandle>();
        audioSource = doorAudioSource;
    }

    // Update is called once per frame
    void Update() {
        if (movedByNPC) { // only for NCPs
            // Check if the rotation is close enough to the target
            if (Mathf.Abs(angle - targetRotation) < angleSnap) {
                movedByNPC = false; // Stop moving
                angle = targetRotation; // Ensure exact alignment
                doorHinge.limits = new JointLimits { min = -angle-0.1f, max = -angle+0.1f }; // Ensure exact alignment
                doorRigidBody.velocity = Vector3.zero; // Stop any residual movement
                doorRigidBody.angularVelocity = Vector3.zero; // Stop any residual rotation
            }
            
            else if (angle > targetRotation) {
                angle -= 1f * doorSpeed * Time.deltaTime;
                doorHinge.limits = new JointLimits { min = -angle-0.1f, max = -angle+0.1f };
            }
            else if (angle < targetRotation) {
                angle += 1f * doorSpeed * Time.deltaTime;
                doorHinge.limits = new JointLimits { min = -angle-0.1f, max = -angle+0.1f };
            }
        }

        else if (!(handleScript.IsGrabbed()) && (doorHinge.limits.max != -closedAngle) && (Mathf.Abs(angle - closedAngle) < angleSnap)) { // this means the player has let go of the door handle
            angle = closedAngle; // Ensure exact alignment
            doorHinge.limits = new JointLimits { min = -angle-0.1f, max = -angle+0.1f }; 
            
            StartCoroutine(closeSoundCoroutine());
        }
        
        else if (handleScript.IsGrabbed()) { // this means the player is holding the door handle
            Vector3 hingeLocation = new Vector3(this.transform.position.x, 0f, this.transform.position.z);
            Vector3 handleLocation = new Vector3(doorHandle.transform.position.x, 0f, doorHandle.transform.position.z);

            Vector3 dir = handleLocation - hingeLocation;

            // Debug.DrawLine(hingeLocation, closedVector, Color.red);
            // Debug.DrawLine(hingeLocation, handleLocation, Color.yellow);
            Debug.DrawLine(hingeLocation, closedVector, Color.red);
            Debug.DrawLine(hingeLocation, handleLocation, Color.yellow);


            // Now calculate the angle between the closedVector and the dir vector
            angle = Vector3.Angle(closedVector, dir);
            // this should always be between 0-90, so we need to flip the sign later in the limits
            
            //float angle = Vector3.Angle(doorHinge.anchor, doorHandle.transform.position);
            if (angle >= closedAngle && angle <= openAngle) { // if its swinging the right way, move it
                doorHinge.limits = new JointLimits { min = -angle-0.1f, max = -angle+0.1f };
                doorRigidBody.velocity = Vector3.zero; // Stop any residual movement
                doorRigidBody.angularVelocity = Vector3.zero; // Stop any residual rotation
            }
        }

        else if (swinging) {
            if (angle >= closedAngle && angle <= openAngle) {
                doorHinge.limits = new JointLimits { min = -angle-0.1f, max = -angle+0.1f };
                doorRigidBody.velocity = Vector3.zero; // Stop any residual movement
                doorRigidBody.angularVelocity = Vector3.zero; // Stop any residual rotation
            }
        }
    }


    // NPC STUFF
    // These two functions directly change the doors target angle, which is will move toward.
    public void Open() {
        targetRotation = openAngle;
        movedByNPC = true;
        StartCoroutine(openSoundCoroutine());
    }

    public void Close() {
        targetRotation = closedAngle;
        movedByNPC = true;
        StartCoroutine(closeSoundCoroutine());
    }


    // Open the door for a set amount of time
    public void OpenDoorTemporarily(float timeOpen) {
        if (temporaryOpenCoroutine != null) { // ensures it is not already in the process of opening/closing
            StopCoroutine(temporaryOpenCoroutine);
        }
        temporaryOpenCoroutine = StartCoroutine(TemporaryOpenCoroutine(timeOpen));
    }

    private IEnumerator TemporaryOpenCoroutine(float timeOpen) {
        Open();
        yield return new WaitForSeconds(timeOpen);
        Close();
        temporaryOpenCoroutine = null;
    }


    // These two functions are used to play the door sound effect
    private IEnumerator closeSoundCoroutine() {
        yield return new WaitForSeconds(closeOffset);
        doorAudioSource.clip = closeDoorClip;
        doorAudioSource.Play();
    }
    private IEnumerator openSoundCoroutine() {
        yield return new WaitForSeconds(openOffset);
        doorAudioSource.clip = openDoorClip;
        doorAudioSource.Play();
    }

    public IEnumerator SwingDoor(Vector3 handleVelocity) {
        swinging = true;
        float velocityMagnitude = handleVelocity.magnitude*doorswingMultiplier;
        float velocityThreshold = 0.25f; // Adjust this threshold as needed

        // If the velocity is below the threshold, do not swing the door
        if (velocityMagnitude < velocityThreshold) {
            swinging = false;
            yield break;
        }
        
        float swingDuration = Mathf.Clamp(Mathf.Abs(velocityMagnitude), 0.5f, 3f); // Clamp duration between 0.5 and 3 seconds
        // Determine the swing projection
        float velDoorAngle = Mathf.Abs(Vector3.Angle(handleVelocity, -this.transform.right));
        float dir = 1f;
        if (velDoorAngle > 90f) {
            velDoorAngle = 180f - velDoorAngle;
            dir = -1f;
        }
        // Use the inverse of the angle between to determine the swing speed
        float inverseAngleFactor = 90f-velDoorAngle;
        
        // Determine the target swing angle based on the velocity magnitude and the inverse angle factor
        float targetSwingAngle = Mathf.Clamp(angle + (dir * velocityMagnitude * inverseAngleFactor * swingMultiplier), closedAngle, openAngle);
        
        float initialAngle = angle;
        float elapsedTime = 0f;

        while (elapsedTime < swingDuration) {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / swingDuration;
            angle = Mathf.Lerp(initialAngle, targetSwingAngle, 1 - (1 - t) * (1 - t)); // Ease out

            yield return null;
        }
        swinging = false;

        // Ensure exact alignment at the end
        angle = targetSwingAngle;
    }
}