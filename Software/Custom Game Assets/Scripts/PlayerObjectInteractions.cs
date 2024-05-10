using System.Collections;
using System.Collections.Generic;
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
        if (Input.GetKey(KeyCode.E) && player.isBlocked(player.transform.forward) && player.lastBlockedByType == "Doors") {
            GameObject door = player.lastBlockedByObj;
            if (door.tag == "Closed") {
                door.transform.Rotate(0, -90, 0);
                door.tag = "Open";
            }
            else if (door.tag == "Open") {
                door.transform.Rotate(0, 90, 0);
                door.tag = "Closed";
            }
            else {
                throw new UnityException("Door must start with a tag, either \"Open\" or \"Closed\"");
            }
        }
    }
}
