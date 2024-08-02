using UnityEngine;

public class Alarm : Audible
{
    [SerializeField] public AudioSource alarmSource;
    [SerializeField] private AudioClip alarmClip;
    [SerializeField] public GameObject fire;

    private void Start() {
        alarmSource.clip = alarmClip;
        
    }
    private void Update() {
        if (fire.active && !alarmSource.isPlaying) {
            alarmSource.Play();
        }
        else if (!fire.active && alarmSource.isPlaying) {
            alarmSource.Stop();
        }
        
    }
}
