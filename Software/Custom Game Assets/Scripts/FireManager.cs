using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireManager : Audible {
    [SerializeField] private float fireDelay = 1f;
    [SerializeField] private AudioSource fireAudioSource;
    private void OnEnable() {
        audioSource = fireAudioSource;
        StartCoroutine(ExplosionDelayCoroutine());
    } 

    private IEnumerator ExplosionDelayCoroutine() {
        yield return new WaitForSeconds(fireDelay);
        Debug.Log("playing fire sound");
        fireAudioSource.Play();
    }
}
