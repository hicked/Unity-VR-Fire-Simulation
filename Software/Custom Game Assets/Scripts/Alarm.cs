using UnityEngine;
using System.Collections;
public class Alarm : Audible
{
    [SerializeField] public AudioSource alarmSource;
    [SerializeField] private AudioClip alarmClip;
    [SerializeField] public GameObject fire;
    [SerializeField] private float alarmDelay = 4f;
    private FireManager fireManager;
    [SerializeField] public bool isPlaying = false;

    private void Start() {
        fireManager = fire.GetComponent<FireManager>();
        alarmSource.clip = alarmClip;
    }

    private void Update() {
        if (fireManager.fireParticleSystem.isPlaying && !this.isPlaying) {
            StartCoroutine(delaySound(alarmDelay));
        }
        else if (!fireManager.fireParticleSystem.isPlaying && this.isPlaying) {
            alarmSource.Stop();
            this.isPlaying = false;
        }
        
    }
    private IEnumerator delaySound(float delay) {
        alarmSource.Play(); // for some reason there is a delay before the sound actually plays
        yield return new WaitForSeconds(delay);
        this.isPlaying = true;
    }
}
