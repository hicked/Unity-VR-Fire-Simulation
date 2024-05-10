using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Player.cs;

public class PlayerObjectInteractions : MonoBehaviour
{
    private Player player;
    private Animator doorAnimator;
    private Rigidbody doorRigidbody;
    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && player.isBlocked(player.transform.forward) && player.lastBlockedByType == "Doors") {
            GameObject door = player.lastBlockedByObj;
            doorAnimator = door.GetComponent<Animator>();
            doorRigidbody = door.GetComponent<Rigidbody>();

            if (doorAnimator.GetBool("IsClosed") == true) { // technically WAS closed
                doorAnimator.SetBool("IsOpen", true);
                doorAnimator.SetBool("IsClosed", false);
            }
            else if (doorAnimator.GetBool("IsOpen") == true) {
                doorAnimator.SetBool("IsClosed", true);
                doorAnimator.SetBool("IsOpen", false);
            }
        }
    }
}
