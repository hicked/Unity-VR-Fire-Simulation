using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;

public class CandleInteractable : XRGrabInteractable {
    private ParticleSystem flameParticleSystem;
    private Light flameLight;
    private AudioSource flameAudioSource;

    protected void Start() {
        base.Awake();
        flameAudioSource = GetComponent<AudioSource>();
        flameParticleSystem = GetComponent<ParticleSystem>();
        flameLight = GetComponentInChildren<Light>();
    }

    [System.Obsolete("Apparently this is obsolete")]
    protected override void OnSelectEntering(XRBaseInteractor interactor) {   
        StartCoroutine(ExtinguishFlame());
    }

    private IEnumerator ExtinguishFlame() {
        flameAudioSource.Play();
        flameParticleSystem.Stop();
        flameLight.enabled = false;

        // Wait until the audio has finished playing
        yield return new WaitForSeconds(flameAudioSource.clip.length);
        gameObject.SetActive(false);
    }
}
