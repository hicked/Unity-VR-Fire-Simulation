using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;

public class DoorHandle : XRGrabInteractable {
    [SerializeField] private XRGrabInteractable interactor;
    [SerializeField] private float maxDistanceFromHandle = 0.5f;

    [SerializeField] private GameObject lockedDoorMessage;
    [SerializeField] public AudioClip lockedDoorClip;
    [SerializeField] private float lockedDoorMessageFadeIn = 0.5f;
    [SerializeField] private float lockedDoorMessageTime = 2f;
    [SerializeField] private float lockedDoorMessageFadeOut = 0.5f;
    public bool isLocked = false; // Flag to track if door is locked
    private bool isMessageDisplaying = false; // Flag to track if message is already displaying

    [SerializeField] private AudioSource doorAudioSource;
    [SerializeField] private GameObject handle;
    [SerializeField] private GameObject door;
    private Rigidbody doorRigidBody;
    private Rigidbody handleRigidBody;

    override protected void Awake(){
        doorRigidBody = door.GetComponent<Rigidbody>();
        handleRigidBody = handle.GetComponent<Rigidbody>();
        interactor.selectEntered.AddListener(OnGrab);
        interactor.selectExited.AddListener(OnRelease);
        base.Awake();
    }

    void FixedUpdate() {
        if (!IsGrabbed() && Vector3.Distance(this.transform.position, handle.transform.position) > 0.1f) {
            ResetPosition();
        }
        else if (IsGrabbed() && Vector3.Distance(this.transform.position, handle.transform.position) > maxDistanceFromHandle) {
            ForceDrop();
        }
    }

    public void ForceDrop() {
        interactor.enabled = false;
        ResetPosition();
        StartCoroutine(ReenableGrabInteractable()); // adds delay to reenable grabInteractable
    }

    public void ResetPosition() {
        doorRigidBody.velocity = Vector3.zero;
        doorRigidBody.angularVelocity = Vector3.zero;
        handleRigidBody.velocity = Vector3.zero;
        handleRigidBody.angularVelocity = Vector3.zero;
        this.transform.position = handle.transform.position; 
        this.transform.rotation = handle.transform.rotation;
    }

    public bool IsGrabbed() {
        return interactor.isSelected;
    }

    override protected void OnDestroy() {
        interactor.selectEntered.RemoveListener(OnGrab);
        interactor.selectExited.RemoveListener(OnRelease);
    }

    private void OnGrab(SelectEnterEventArgs args) {
        Debug.Log("Object Grabbed");

        if (isLocked) {
            interactor.enabled = false; // Force drop
            ResetPosition();
            StartCoroutine(ReenableGrabInteractable()); // Reenable after a short delay

            if (!isMessageDisplaying) {
                doorAudioSource.clip = lockedDoorClip;
                doorAudioSource.Play();
                StartCoroutine(ShowLockedDoorMessage(lockedDoorMessageFadeIn, lockedDoorMessageTime, lockedDoorMessageFadeOut));
            }
        }
    }

    private void OnRelease(SelectExitEventArgs args) {
        Debug.Log("Object Released");

        ResetPosition();
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

    private IEnumerator ReenableGrabInteractable() {
        yield return new WaitForSeconds(0.1f);
        interactor.enabled = true;
    }
}
