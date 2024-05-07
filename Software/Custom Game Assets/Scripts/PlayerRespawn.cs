using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    [SerializeField] private float angleThreshold = 90f;
    [SerializeField] private float spawnX = 0f;
    [SerializeField] private float spawnY = 1f;
    [SerializeField] private float spawnZ = 0f;

    // Update is called once per frame
    private void Update()
    {     
        Quaternion rotation = transform.rotation;
        if (((rotation.eulerAngles.x) > angleThreshold && (rotation.eulerAngles.x) < (360 - angleThreshold)) || // Between thresholds, ex: 360 and 90
            ((rotation.eulerAngles.z) > angleThreshold && (rotation.eulerAngles.z) < (360 - angleThreshold))) {
            
            Vector3 originPositionVector = new Vector3(spawnX, spawnY, spawnZ); //repawns at 0, 0; can change this later
            transform.position = originPositionVector;

            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
    }
}
