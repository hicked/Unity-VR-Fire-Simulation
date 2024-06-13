using System.Collections;
using UnityEngine;
using TMPro;

public class Doors : MonoBehaviour, Interactable {
    [SerializeField] public AudioSource doorAudioSource;
    [SerializeField] public AudioClip openDoorClip;
    [SerializeField] public AudioClip closeDoorClip;
    [SerializeField] public AudioClip lockedDoorClip;
    [SerializeField] public float closeOffset = 0.55f;
    [SerializeField] public float openOffset = 0.55f;
    [SerializeField] private float doorSpeed = 90f;

    [SerializeField] private float lockedDoorMessageFadeIn = 0.5f;
    [SerializeField] private float lockedDoorMessageTime = 2f;
    [SerializeField] private float lockedDoorMessageFadeOut = 0.5f;

    public GameObject lockedDoorMessage; // Reference to UI element showing the locked door message
    private bool isMessageDisplaying; // Flag to track if message is already displaying

    private Quaternion targetRotation;
    private float rotationOpen;
    private float rotationClosed;
    public bool isOpen = false;
    public bool isMoving = false;
    public bool isLocked;
    private Coroutine temporaryOpenCoroutine;

    public void Interact() {
        if (isLocked) {
            if (!isMessageDisplaying) {
                doorAudioSource.clip = lockedDoorClip;
                doorAudioSource.Play();
                StartCoroutine(ShowLockedDoorMessage(lockedDoorMessageFadeIn, lockedDoorMessageTime, lockedDoorMessageFadeOut));
            }
        }
        else {
            if (isOpen) {
                Close();
            }
            else {
                Open();
            }
        }
    }

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
        doorAudioSource.clip = closeDoorClip;
        doorAudioSource.Play();
    }
    private IEnumerator openCoroutine() {
        yield return new WaitForSeconds(openOffset);
        doorAudioSource.clip = openDoorClip;
        doorAudioSource.Play();
    }
    
    private IEnumerator ShowLockedDoorMessage(float fadeInDuration, float displayDuration, float fadeOutDuration) {
        isMessageDisplaying = true;
        TextMeshProUGUI messageText = lockedDoorMessage.GetComponent<TextMeshProUGUI>();
        lockedDoorMessage.SetActive(true);
        Color originalColor = messageText.color;
        originalColor.a = 0f; // Start with fully transparent

        // Fade in
        float timer = 0f;
        while (timer < fadeInDuration) {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, timer / fadeInDuration);
            messageText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        // Hold for display duration
        yield return new WaitForSeconds(displayDuration);

        // Fade out
        timer = 0f;
        while (timer < fadeOutDuration) {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, timer / fadeOutDuration);
            messageText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        // Ensure fully transparent at the end
        messageText.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);

        // Hide the message
        lockedDoorMessage.SetActive(false);
        isMessageDisplaying = false;
    }
}