using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class CandleInteractable : XRGrabInteractable {
    private ParticleSystem flameParticleSystem;
    public float pinchThreshold = 0.02f;

    protected override void Awake() {
        base.Awake();
        flameParticleSystem = GetComponent<ParticleSystem>();
    }

    [System.Obsolete("Apparently this is obsolete")]
    protected override void OnSelectEntering(XRBaseInteractor interactor) {   
        ExtinguishFlame();
    }

    private void ExtinguishFlame() {
        if (flameParticleSystem != null && flameParticleSystem.isPlaying) {
            flameParticleSystem.Stop();
        }
    }
}
