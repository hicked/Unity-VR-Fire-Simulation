using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerObjectInteractions : MonoBehaviour {
    private Player player;
    [SerializeField] private float interactorLength = 1f;

    // Start is called before the first frame update
    void Start() {
        player = GetComponent<Player>();
    }

    // Update is called once per frame
    void Update() {
        // IMPORTANT: this is a really bad way to do it, since the lastBlocked object is from a dif length than the interactor length, but were doing vr so its temporary
        if (Input.GetKeyDown(KeyCode.E) && player.isBlocked(player.transform.forward, interactorLength)) {
            player.lastBlockedByObj.GetComponent<Interactable>()?.KeyboardInteract();
        }
    }
}