using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class FireManager : Audible {
    [SerializeField] private float fireDelay = 1f;
    [SerializeField] public Light fireLight;
    [SerializeField] private AudioSource fireAudioSource;
    [SerializeField] private AudioSource explosionAudioSource;
    [SerializeField] public ParticleSystem fireParticleSystem;
    [SerializeField] private ParticleSystem explosionParticleSystem;
    [SerializeField] private int minTimeBeforeInstantiation = 10;
    [SerializeField] private int maxTimeBeforeInstantiation = 30;
    private int waitTime;

    private IEnumerator startFire() {    
        yield return new WaitForSeconds(waitTime);
        fireLight.enabled = true;
        explosionAudioSource.Play();
        explosionParticleSystem.Play();
        yield return new WaitForSeconds(fireDelay);
        fireAudioSource.Play();
        fireParticleSystem.Play();
    }
    void Start () {
        audioSource = fireAudioSource; // from audible class
        waitTime = Random.Range(minTimeBeforeInstantiation, maxTimeBeforeInstantiation);
        StartCoroutine(startFire());
    }

    void Update () {
        
    }

}
