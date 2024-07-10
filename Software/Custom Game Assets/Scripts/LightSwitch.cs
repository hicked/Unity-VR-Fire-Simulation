using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSwitch : MonoBehaviour, Interactable {
    [SerializeField] private List<Light> pointLights;
    [SerializeField] private List<AudioClip> switchSounds;
    [SerializeField] public AudioSource switchAudioSource;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(blah());
    }

    // Update is called once per frame
    void Update() {}
    public void GrabInteract(GameObject hand) {}
    public void PinchInteract() {}
    public void PokeInteract() {}

    public void KeyboardInteract() {
        if (pointLights.Count == 0) {
            //Debug.LogError("No lights attached to light switch");
            return;
        }

        switchAudioSource.clip = switchSounds[Random.Range(0, switchSounds.Count)];
        switchAudioSource.Play();
        if (pointLights[0].gameObject.activeInHierarchy) {
            foreach (Light pointLight in pointLights) {
                pointLight.gameObject.SetActive(false);
            }
        }
        else {
            foreach (Light pointLight in pointLights) {
                pointLight.gameObject.SetActive(true);
            }
        }
        transform.Rotate(transform.up, 180, Space.World);
    }

    private IEnumerator blah() {
        while (true) {
            yield return new WaitForSeconds(10);
            KeyboardInteract();
        }
    }
}
