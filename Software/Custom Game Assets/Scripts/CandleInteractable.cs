using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;

public class CandleInteractable : XRGrabInteractable {
    [SerializeField] private float audioOffset = 0.25f; // time after audio plays that the light turns off
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
        yield return new WaitForSeconds(audioOffset);
        flameParticleSystem.Stop();
        flameParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear); // Stop and clear particles immediately
        flameLight.enabled = false;
        
        // Wait until the audio has finished playing
        yield return new WaitForSeconds(Mathf.Max(flameAudioSource.clip.length - audioOffset, 0f));
        gameObject.SetActive(false);
    }
}
