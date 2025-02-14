using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : MonoBehaviour {
    [SerializeField] private GameObject fire;
    [SerializeField] private GameObject[] NPCs;
    [SerializeField] private GameObject player;
    [SerializeField] public bool isActive = false;
    [SerializeField] private float distance = 25f;
    public int numNPCHit = 0;
    public bool playerHit = false;
    private BoxCollider boxCollider;


    // Start is called before the first frame update
    void Start() {
        boxCollider = GetComponent<BoxCollider>();
    }
    void Update() {
        numNPCHit = 0;
        foreach (GameObject npc in NPCs) {
            try {
                RaycastHit npcHit; // hit info for raycast
                Vector3 NPCHeightOffset = new Vector3(0, npc.GetComponent<NPCManager>().NPCHeight/2, 0);
                
                if (Physics.Raycast(fire.transform.position + NPCHeightOffset, (npc.transform.position + NPCHeightOffset) - (fire.transform.position + NPCHeightOffset), out npcHit, distance)) {
                    if (npcHit.collider.gameObject == npc) {
                        numNPCHit++;
                    }
                }
            }
            catch {
                continue;
            }
            
        }

        RaycastHit playerRaycastHit; // hit info for raycast
        Vector3 playerHeightOffset = new Vector3(0, player.GetComponent<BoxCollider>().size.y/2, 0);
        if (Physics.Raycast(fire.transform.position + playerHeightOffset, player.transform.position - (fire.transform.position + playerHeightOffset), out playerRaycastHit, distance)) {
            if (playerRaycastHit.collider.gameObject == player) {
                playerHit = true;
            }
            else {
                playerHit = false;
            }
        }

        if (numNPCHit == 0 && !playerHit) {
            boxCollider.enabled = true;
        }
        else {
            boxCollider.enabled = false;
        }
    }
}
