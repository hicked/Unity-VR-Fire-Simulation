using System.Collections;
using UnityEngine;

public class Doors : MonoBehaviour {
    [SerializeField] public AudioSource audioSource;
    [SerializeField] public AudioClip openDoorClip;
    [SerializeField] public AudioClip closeDoorClip;
    [SerializeField] public AudioClip lockedDoorClip;
    [SerializeField] public float closeOffset = 0.55f;
    [SerializeField] public float openOffset = 0.55f;
    [SerializeField] private float doorSpeed = 90f;

    private Quaternion targetRotation;
    private float rotationOpen;
    private float rotationClosed;
    public bool isOpen = false;
    public bool isMoving = false;
    public bool isLocked;
    private Coroutine temporaryOpenCoroutine;

    // Start is called before the first frame update
    void Start() {
        rotationClosed = transform.eulerAngles.y;
        rotationOpen = rotationClosed - 90f;
        targetRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update() {
        if (isMoving) {
            // Smoothly rotate towards the target rotation
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, doorSpeed * Time.deltaTime);
            // Check if the rotation is close enough to the target
            if (Quaternion.Angle(transform.rotation, targetRotation) < 0.1f) {
                isMoving = false; // Stop moving
                transform.rotation = targetRotation; // Ensure exact alignment
            }
        }
    }

    public void Open() {
        targetRotation = Quaternion.Euler(transform.eulerAngles.x, rotationOpen, transform.eulerAngles.z);
        isOpen = true;
        isMoving = true;
        StartCoroutine(openCoroutine());
    }

    public void Close() {
        targetRotation = Quaternion.Euler(transform.eulerAngles.x, rotationClosed, transform.eulerAngles.z);
        isOpen = false;
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
    }
    private IEnumerator closeCoroutine() {
        yield return new WaitForSeconds(closeOffset);
        audioSource.clip = closeDoorClip;
        audioSource.Play();
    }
    private IEnumerator openCoroutine() {
        yield return new WaitForSeconds(openOffset);
        audioSource.clip = openDoorClip;
        audioSource.Play();
    }
}