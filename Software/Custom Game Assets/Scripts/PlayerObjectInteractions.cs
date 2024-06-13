using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;

public class PlayerObjectInteractions : MonoBehaviour {
    private Player player;

    public GameObject lockedDoorMessage; // Reference to UI element showing the locked door message
    private bool isMessageDisplaying; // Flag to track if message is already displaying
    [SerializeField] private float lockedDoorMessageFadeIn = 0.5f;
    [SerializeField] private float lockedDoorMessageTime = 2f;
    [SerializeField] private float lockedDoorMessageFadeOut = 0.5f;
    [SerializeField] private float interactorLength = 1f;

    // Start is called before the first frame update
    void Start() {
        player = GetComponent<Player>();
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.E) && player.isBlocked(player.transform.forward, interactorLength) && player.lastBlockedByType == "Doors") {
            Doors door = player.lastBlockedByObj.GetComponent<Doors>();
            if (!door.isLocked) {
                if (door.isOpen) {
                    door.Close();
                }
                else {
                    door.Open();
                }
            }
            else {
                if (!isMessageDisplaying) {
                    door.audioSource.clip = door.lockedDoorClip;
                    door.audioSource.Play();
                    StartCoroutine(ShowLockedDoorMessage(lockedDoorMessageFadeIn, lockedDoorMessageTime, lockedDoorMessageFadeOut));
                }
            }
        }
    }

    IEnumerator ShowLockedDoorMessage(float fadeInDuration, float displayDuration, float fadeOutDuration) {
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


