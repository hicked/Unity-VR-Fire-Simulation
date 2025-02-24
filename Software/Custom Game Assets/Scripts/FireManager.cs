using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class FireManager : Audible {    
    [SerializeField] public List<GameObject> roomDoors;
    [SerializeField] private float fireDelay = 1f;
    [SerializeField] public Light fireLight;
    [SerializeField] private AudioSource fireAudioSource;
    [SerializeField] private AudioSource explosionAudioSource;
    [SerializeField] public ParticleSystem fireParticleSystem;
    [SerializeField] private ParticleSystem explosionParticleSystem;
    [SerializeField] private int minTimeBeforeInstantiation = 10;
    [SerializeField] private int maxTimeBeforeInstantiation = 30;
    [SerializeField] private GameObject player;
    [SerializeField] private float distance = 25f;
    private int waitTime;

    private IEnumerator startFire() {    
        while (true) {
            yield return new WaitForSeconds(waitTime);
            RaycastHit playerRaycastHit; // hit info for raycast
            Vector3 playerHeightOffset = new Vector3(0, player.GetComponent<BoxCollider>().size.y / 2, 0);
            
            if (Physics.Raycast(this.transform.position + playerHeightOffset, player.transform.position - (this.transform.position + playerHeightOffset), out playerRaycastHit, distance)) {
                if (playerRaycastHit.collider.gameObject != player) {
                    foreach (GameObject door in roomDoors) {
                        door.GetComponent<Doors>().Open();
                    }
                    fireLight.enabled = true;
                    explosionAudioSource.Play();
                    explosionParticleSystem.Play();
                    yield return new WaitForSeconds(fireDelay);
                    fireAudioSource.Play();
                    fireParticleSystem.Play();
                    break;
                }
            }
        }
    }
    void Start () {
        audioSource = fireAudioSource; // from audible class
        waitTime = Random.Range(minTimeBeforeInstantiation, maxTimeBeforeInstantiation);
        StartCoroutine(startFire());
    }

    void Update () {
        
    }

}
