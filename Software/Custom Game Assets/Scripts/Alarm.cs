using UnityEngine;

public class Alarm : Audible
{
    [SerializeField] public AudioSource alarmSource;
    [SerializeField] private AudioClip alarmClip;
    [SerializeField] public GameObject fire;
    private FireManager fireManager;

    private void Start() {
        fireManager = fire.GetComponent<FireManager>();
        alarmSource.clip = alarmClip;
    }
    private void Update() {
        if (fireManager.fireParticleSystem.isPlaying && !alarmSource.isPlaying) {
            alarmSource.Play();
        }
        else if (fireManager.fireParticleSystem.isPlaying && alarmSource.isPlaying) {
            alarmSource.Stop();
        }
        
    }
}
