using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Doors : Audible {
    [SerializeField] public AudioSource doorAudioSource;
    [SerializeField] public AudioClip openDoorClip;
    [SerializeField] public AudioClip closeDoorClip;

    [SerializeField] public float closeOffset = 0.55f;
    [SerializeField] public float openOffset = 0.55f;
    [SerializeField] private float doorSpeed = 90f;

    [SerializeField] private float angleSnap = 5f; // door will snap closed if the angle is less than this
    [SerializeField] public GameObject doorHandle;
    private DoorHandle handleScript;
    [SerializeField] private HingeJoint doorHinge;
    private Rigidbody doorRigidBody;
    

    // used for npcs opening doors, not player
    private Quaternion targetRotation;
    private float rotationOpen;
    private float rotationClosed;
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


    // public void KeyboardInteract() {
    //     if (isLocked) {
    //         if (!isMessageDisplaying) {
    //             doorAudioSource.clip = lockedDoorClip;
    //             doorAudioSource.Play();
    //             StartCoroutine(ShowLockedDoorMessage(lockedDoorMessageFadeIn, lockedDoorMessageTime, lockedDoorMessageFadeOut));
    //         }
    //     }
    //     else {
    //         if (isOpen) {
    //             Close();
    //         }
    //         else {
    //             Open();
    //         }
    //     }
    // }

    // Start is called before the first frame update
    void Start() {
        doorRigidBody = GetComponent<Rigidbody>();
        handleScript = doorHandle.GetComponent<DoorHandle>();
        audioSource = doorAudioSource;
        rotationClosed = transform.eulerAngles.y;
        rotationOpen = rotationClosed - 90f;
    }

    // Update is called once per frame
    void Update() {
        /*if (isMoving) { // only for NCPs
            Debug.Log("ISMOVING");
            // Smoothly rotate towards the target rotation
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, doorSpeed * Time.deltaTime);
            doorHandle.transform.position = this.transform.position + doorHandleVector; // handle tracks door while npc opens it
            // Check if the rotation is close enough to the target
            if (Quaternion.Angle(transform.rotation, targetRotation) < 0.1f) {
                isMoving = false; // Stop moving
                transform.rotation = targetRotation; // Ensure exact alignment
            }
        }

        else 
        */

        if (!(handleScript.IsGrabbed())) { // this means the player has let go of the door handle
            if (Mathf.Abs(doorHinge.angle - doorHinge.limits.max) < angleSnap) { // door is closed
                handleScript.ForceDrop();
                StartCoroutine(closeCoroutine());
                transform.rotation = Quaternion.Euler(transform.eulerAngles.x, rotationClosed, transform.eulerAngles.z);;
            }
        }
    }



    // NPC STUFF
    public void Open() {
        targetRotation = Quaternion.Euler(transform.eulerAngles.x, rotationOpen, transform.eulerAngles.z);
        isMoving = true;
        StartCoroutine(openCoroutine());
    }

    public void Close() {
        targetRotation = Quaternion.Euler(transform.eulerAngles.x, rotationClosed, transform.eulerAngles.z);
        isMoving = true;
        StartCoroutine(closeCoroutine());
    }

    public void OpenDoorTemporarily(float timeOpen) {
        if (temporaryOpenCoroutine != null) {
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
    private IEnumerator closeCoroutine() {
        yield return new WaitForSeconds(closeOffset);
        doorAudioSource.clip = closeDoorClip;
        doorAudioSource.Play();
    }
    private IEnumerator openCoroutine() {
        yield return new WaitForSeconds(openOffset);
        doorAudioSource.clip = openDoorClip;
        doorAudioSource.Play();
    }
}