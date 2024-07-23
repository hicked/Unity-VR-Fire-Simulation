using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;

public class CandleInteractable : XRGrabInteractable {
    private ParticleSystem flameParticleSystem;
    private Light flameLight;

    protected void Start() {
        base.Awake();
        flameParticleSystem = GetComponent<ParticleSystem>();
        flameLight = GetComponentInChildren<Light>();
    }

    [System.Obsolete("Apparently this is obsolete")]
    protected override void OnSelectEntering(XRBaseInteractor interactor) {   
        ExtinguishFlame();
    }

    private void ExtinguishFlame() {
        gameObject.SetActive(false);
    }
}
