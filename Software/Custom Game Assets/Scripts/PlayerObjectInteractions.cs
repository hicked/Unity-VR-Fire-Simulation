using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
//using Player.cs;

public class PlayerObjectInteractions : MonoBehaviour
{
    private Player player;
    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && player.isBlocked(player.transform.forward) && player.lastBlockedByType == "Doors") {
            Doors door = player.lastBlockedByObj.GetComponent<Doors>();
            if (door.isOpen) {
                door.isOpen = false;
            }
            else {
                door.isOpen = true;
            }
        }
    }
}
