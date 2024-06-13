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
        if (Input.GetKeyDown(KeyCode.E) && player.isBlocked(player.transform.forward, interactorLength)) {
            player.lastBlockedByObj.GetComponent<Interactable>()?.Interact();
        }
    }
}