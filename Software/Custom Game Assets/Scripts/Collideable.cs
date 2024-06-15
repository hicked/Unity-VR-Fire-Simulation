using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collideable : MonoBehaviour {
    // All scripts for which collisions will be checked must extend Collideable
    // May not be that useful after all. For now commented
    /*
    public bool PerformCapsuleCast(Vector3 location, Vector3 dir, float distance, int layerMask, out RaycastHit hitInfo) {
        return Physics.CapsuleCast(
            new Vector3(location.x, location.y + NPCHeight / 2f, location.z),
            new Vector3(location.x, location.y - NPCHeight / 2f, location.z),
            NPCRadius * 1.2f,
            dir,
            out hitInfo,
            distance,
            layerMask
        );
    }*/
}
