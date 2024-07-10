using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowDoorHandle : MonoBehaviour
{
    [SerializeField] public GameObject interactableHandle;
    Rigidbody doorHandleRigidbody;
    // Start is called before the first frame update
    void Start()
    {
        doorHandleRigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate() {
        if (Vector3.Distance(this.transform.position, interactableHandle.transform.position) > 0.1f) {
            doorHandleRigidbody.MovePosition(interactableHandle.transform.position);
        }
    }
}
