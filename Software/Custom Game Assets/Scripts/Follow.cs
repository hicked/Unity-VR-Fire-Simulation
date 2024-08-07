using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Follow : MonoBehaviour {
    [SerializeField] public GameObject followObject;
    void FixedUpdate() {
        // Debug.Log(interactableHandle.GetComponent<DoorHandle>().IsGrabbed());
        if (Vector3.Distance(this.transform.position, followObject.transform.position) > 0.1f) {
            this.transform.position = followObject.transform.position;
        }
    }
}