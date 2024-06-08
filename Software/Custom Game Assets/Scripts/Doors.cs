/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Doors : MonoBehaviour {
    [SerializeField] private float doorSpeed = 100f;
    [SerializeField] private float timeSpentOpenByNPC = 1.5f;
    private float initialAngleY;
    public bool isOpen = false;
    public string openedBy;
    public float timeSinceOpenedByNPC;
    // Start is called before the first frame update
    void Start()
    {
        initialAngleY = this.transform.eulerAngles.y;
    }
    
    // Update is called once per frame
    void Update() {
        if (isOpen && openedBy == "NPC") {
            if  (timeSinceOpenedByNPC > timeSpentOpenByNPC) {
                isOpen = false;
            }
            else {
                timeSinceOpenedByNPC += Time.deltaTime;
            }
        }
        int direction = 1;
        float targetAngleY = initialAngleY - 90f * (isOpen ? 1f : 0f);
        float currentAngleY = this.transform.eulerAngles.y;

        if (targetAngleY < 0) {
            targetAngleY += 360; // Adjust to be in range [0, 360)
        }
        if (Mathf.Abs(targetAngleY - currentAngleY) > 180) {
            direction *= -1; // Reverse direction for angles larger than 180 degrees
        }
    
        currentAngleY = (currentAngleY + 360) % 360;
        targetAngleY = (targetAngleY + 360) % 360;
        if (Mathf.Round(this.transform.eulerAngles.y) != Mathf.Round(targetAngleY)) {
            this.transform.eulerAngles = new Vector3(this.transform.eulerAngles.x, 
            this.transform.eulerAngles.y + doorSpeed * direction * Time.deltaTime * ((this.transform.eulerAngles.y < targetAngleY) ? 1 : -1), 
            this.transform.eulerAngles.z);
        }
    }
}
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doors : MonoBehaviour {
    [SerializeField] private float doorSpeed = 100f;
    [SerializeField] private float timeSpentOpenByNPC = 1.5f;
    private float currentAngleY;
    private float angleYOpen;
    private float angleYClosed;
    public bool isOpen = false;
    private Coroutine temporaryOpenCoroutine;

    // Start is called before the first frame update
    void Start() {
        angleYClosed = transform.eulerAngles.y;
        angleYOpen = angleYClosed - 90f;
    }

    // Update is called once per frame
    void Update() {
        currentAngleY = transform.eulerAngles.y;
        

        if (isOpen && currentAngleY != angleYOpen) {
            OpenDoor();
        } else if (!isOpen && currentAngleY != angleYClosed){
            CloseDoor();
        }
    }

    public void OpenDoor() {
        float targetAngleY = angleYClosed - 90f;
        RotateDoor(targetAngleY);
        isOpen = true;
    }

    public void CloseDoor() {
        float targetAngleY = angleYClosed;
        RotateDoor(targetAngleY);
        isOpen = false;
    }

    public void OpenDoorTemporarily() {
        if (temporaryOpenCoroutine != null) {
            StopCoroutine(temporaryOpenCoroutine);
        }
        temporaryOpenCoroutine = StartCoroutine(TemporaryOpenCoroutine());
    }

    private IEnumerator TemporaryOpenCoroutine() {
        OpenDoor();
        yield return new WaitForSeconds(timeSpentOpenByNPC);
        CloseDoor();
    }

    private void RotateDoor(float targetAngleY) {
        float currentAngleY = this.transform.eulerAngles.y;

        if (targetAngleY < 0) {
            targetAngleY += 360; // Adjust to be in range [0, 360)
        }

        int direction = 1;
        if (Mathf.Abs(targetAngleY - currentAngleY) > 180) {
            direction *= -1; // Reverse direction for angles larger than 180 degrees
        }

        currentAngleY = (currentAngleY + 360) % 360;
        targetAngleY = (targetAngleY + 360) % 360;

        if (Mathf.Round(this.transform.eulerAngles.y) != Mathf.Round(targetAngleY)) {
            this.transform.eulerAngles = new Vector3(
                this.transform.eulerAngles.x,
                this.transform.eulerAngles.y + doorSpeed * direction * Time.deltaTime * ((this.transform.eulerAngles.y < targetAngleY) ? 1 : -1),
                this.transform.eulerAngles.z
            );
        }
    }
}
