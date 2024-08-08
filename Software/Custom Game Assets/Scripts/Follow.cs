using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Follow : MonoBehaviour {
    [SerializeField] public Transform followObject;
    private Vector3 pos, fw, up;

    void Start() {
        pos = followObject.InverseTransformPoint(transform.position);
        fw = followObject.InverseTransformDirection(transform.forward);
        up = followObject.InverseTransformDirection(transform.up);
    }
    void Update() {
        Vector3 newpos = followObject.transform.TransformPoint(pos);
        Vector3 newfw = followObject.transform.TransformDirection(fw);
        Vector3 newup = followObject.transform.TransformDirection(up);
        Quaternion newrot = Quaternion.LookRotation(newfw, newup);
        transform.position = newpos;
        transform.rotation = newrot;

        // Debug.Log(interactableHandle.GetComponent<DoorHandle>().IsGrabbed());
        // if (Vector3.Distance(this.transform.position, followObject.transform.position) > 0.1f) {
        //     this.transform.position = followObject.transform.position;
        // }
    }
}