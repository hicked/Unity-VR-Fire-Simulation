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

    [SerializeField] public float openAngle = -90f;
    [SerializeField] public float closedAngle = 0f;
    [SerializeField] private float angleSnap = 10f; // door will snap closed if the angle is less than this
    [SerializeField] public GameObject doorHandle;
    private DoorHandle handleScript;
    [SerializeField] private HingeJoint doorHinge;
    private Rigidbody doorRigidBody; // MIGHT NOT NEED THIS TO RESET VELOCITY OF DOOR MIGHT NEED HINGEJOINT VELOCITY
    private Vector3 doorVector;

    // used for npcs opening doors, not player
    private float targetRotation;
    public float angle;
    public bool isMoving = false;

    
    private Coroutine temporaryOpenCoroutine;


    // void Awake() {
    //     grabInteractable = GetComponent<XRGrabInteractable>();
    //     //doorRigidBody = GetComponent<Rigidbody>();

    //     grabInteractable.selectExited.AddListener(OnRelease);
    // }

    // private void OnDestroy() {
    //     grabInteractable.selectExited.RemoveListener(OnRelease);
    // }

    // private void OnRelease(SelectExitEventArgs args) {
    //     Debug.Log("Object Released");
    //     // Your logic when the object is released
    // }


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
        doorRigidBody = GetComponent<Rigidbody>();
        handleScript = doorHandle.GetComponent<DoorHandle>();
        audioSource = doorAudioSource;
    }

    // Update is called once per frame
    void Update() {
        if (isMoving) { // only for NCPs
            Debug.Log("Door is moving");
            // Check if the rotation is close enough to the target
            if (Mathf.Abs(angle - targetRotation) < 0.1f) {
                isMoving = false; // Stop moving
                angle = targetRotation; // Ensure exact alignment
                doorHinge.limits = new JointLimits { min = angle-0.5f, max = angle+0.5f }; // Ensure exact alignment
                doorRigidBody.velocity = Vector3.zero; // Stop any residual movement
                doorRigidBody.angularVelocity = Vector3.zero; // Stop any residual rotation
            }
            
            else if (angle > targetRotation) {
                angle -= 1f * doorSpeed * Time.deltaTime;
                doorHinge.limits = new JointLimits { min = angle-0.5f, max = angle+0.5f };
            }
            else if (angle < targetRotation) {
                angle += 1f * doorSpeed * Time.deltaTime;
                doorHinge.limits = new JointLimits { min = angle-0.5f, max = angle+0.5f };
            }
        }

        else if (!(handleScript.IsGrabbed()) && (doorHinge.limits.max != closedAngle) && (Mathf.Abs(angle - closedAngle) < angleSnap)) { // this means the player has let go of the door handle
            Debug.Log("Door is closed");
            handleScript.ForceDrop();
            angle = closedAngle; // Ensure exact alignment
            doorHinge.limits = new JointLimits { min = angle, max = angle }; 
            
            StartCoroutine(closeSoundCoroutine());
        }
        
        else if (handleScript.IsGrabbed()) { // this means the player is holding the door handle
            Vector3 hingeLocation = new Vector3(this.transform.position.x, 0f, this.transform.position.z);
            Vector3 handleLocation = new Vector3(doorHandle.transform.position.x, 0f, doorHandle.transform.position.z);

            Vector3 dir = handleLocation - hingeLocation;
            angle = -Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;
            //float angle = Vector3.Angle(doorHinge.anchor, doorHandle.transform.position);
            doorHinge.limits = new JointLimits { min = angle-0.5f, max = angle+0.5f };

        }
    }



    // NPC STUFF
    // These two functions directly change the doors target angle, which is will move toward.
    public void Open() {
        targetRotation = openAngle;
        isMoving = true;
        StartCoroutine(openSoundCoroutine());
    }

    public void Close() {
        targetRotation = closedAngle;
        isMoving = true;
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
}