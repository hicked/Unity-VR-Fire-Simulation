using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSwitch : MonoBehaviour, Interactable {
    [SerializeField] private List<Light> pointLights;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(blah());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Interact() {
        if (pointLights[0].gameObject.activeInHierarchy) {
            foreach (Light pointLight in pointLights) {
                pointLight.gameObject.SetActive(false);
            }
        }
        else {
            foreach (Light pointLight in pointLights) {
                pointLight.gameObject.SetActive(true);
            }
        }
        transform.Rotate(transform.up, 180, Space.World);
    }

    private IEnumerator blah() {
        while (true) {
            yield return new WaitForSeconds(10);
            Interact();
        }
    }
}
