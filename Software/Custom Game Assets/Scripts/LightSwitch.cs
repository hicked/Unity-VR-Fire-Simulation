using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class LightSwitch : XRSimpleInteractable {
    [SerializeField] private List<Light> pointLights;
    [SerializeField] private List<AudioClip> switchSounds;
    [SerializeField] public AudioSource switchAudioSource;

    // Start is called before the first frame update
    void Start() {
        StartCoroutine(blah());
    }

    [System.Obsolete("No idea why this is obsolete, but it works.")]
    override protected void OnHoverEntered(XRBaseInteractor interactor) {
        Interact();
    }

    public void Interact() {
        if (pointLights.Count == 0) {
            //Debug.LogError("No lights attached to light switch");
            return;
        }

        switchAudioSource.clip = switchSounds[Random.Range(0, switchSounds.Count)];
        switchAudioSource.Play();
        if (pointLights[0].gameObject.activeInHierarchy) {
            foreach (Light pointLight in pointLights) {
                pointLight.gameObject.SetActive(false);
            }
        }
        else {
            foreach (Light pointLight in pointLights) {
                pointLight.gameObject.SetActive(true);
            }
        }
        transform.Rotate(transform.up, 180, Space.World);
    }

    private IEnumerator blah() {
        while (true) {
            yield return new WaitForSeconds(10);
            Interact();
        }
    }
}
