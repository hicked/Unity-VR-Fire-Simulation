using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowDoorHandle : MonoBehaviour
{
    [SerializeField] public GameObject interactableHandle;
    [SerializeField] public GameObject door;
    private JointSpring doorHingeSpring;
    private HingeJoint doorHinge;
    Rigidbody doorHandleRigidbody;
    // Start is called before the first frame update
    void Start() {
        doorHinge = door.GetComponent<HingeJoint>();
        // doorHingeSpring = doorHinge.spring;
        // doorHingeSpring.spring = 1000f;
        // doorHingeSpring.damper = 0.5f;
        float angle = -70f;
        JointLimits limits = new JointLimits();
        limits.min = angle + 0.1f;
        limits.max = angle;
        doorHinge.limits = limits;
        
        doorHandleRigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate() {
        // Debug.Log(interactableHandle.GetComponent<DoorHandle>().IsGrabbed());
        if (Vector3.Distance(this.transform.position, interactableHandle.transform.position) > 0.1f && !(interactableHandle.GetComponent<DoorHandle>().IsGrabbed())) {
            float angle = 70f;
            doorHingeSpring.targetPosition = angle;
        }
    }
}
