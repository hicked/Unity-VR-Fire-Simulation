using UnityEngine;
using System.Collections;
public class Alarm : Audible
{
    [SerializeField] public AudioSource alarmSource;
    [SerializeField] private AudioClip alarmClip;
    [SerializeField] public GameObject fire;
    [SerializeField] private float alarmDelay = 10f;
    private FireManager fireManager;

    private void Start() {
        fireManager = fire.GetComponent<FireManager>();
        alarmSource.clip = alarmClip;
    }
    private void Update() {
        if (fireManager.fireParticleSystem.isPlaying && !alarmSource.isPlaying) {
            StartCoroutine(playSound());
        }
        else if (!fireManager.fireParticleSystem.isPlaying && alarmSource.isPlaying) {
            alarmSource.Stop();
        }
        
    }
    private IEnumerator playSound() {
        yield return new WaitForSeconds(alarmDelay);
        alarmSource.Play();
    }
}
