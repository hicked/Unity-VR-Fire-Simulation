using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Doors : MonoBehaviour {
    [SerializeField] private float doorSpeed = 100f;
    private float initialAngleY;
    public bool isOpen = false;

    // Start is called before the first frame update
    void Start()
    {
        initialAngleY = this.transform.eulerAngles.y;
    }

    // Update is called once per frame
    void Update()
    {
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
