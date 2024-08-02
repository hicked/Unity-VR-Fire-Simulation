using UnityEngine;
using System.IO.Ports;
using System.Threading;

public class Audible : MonoBehaviour {
    [SerializeField] private float blockedAudioVolume = 0.5f;
    [SerializeField] private LayerMask audioCollisionLayers;
    protected AudioSource audioSource; // defined in child class
    [SerializeField] public AudioListener audioListener; // defined in parent class (since its always the same listener -> player)

    private Vector3 audioSourcePosition = new Vector3();
    private Vector3 audioListenerPosition = new Vector3();

    void Start() {
        audioSource = GetComponent<AudioSource>();
    }
    void Update() {
        audioSourcePosition = audioSource.transform.position;
        audioListenerPosition = audioListener.transform.position;

        if (Physics.Linecast(audioSourcePosition, audioListenerPosition, audioCollisionLayers)) {
            audioSource.volume = blockedAudioVolume; // Adjust the volume as needed
        }
        else {
            audioSource.volume = 1.0f; // Reset the volume if there are no walls in between
        }
    }
}