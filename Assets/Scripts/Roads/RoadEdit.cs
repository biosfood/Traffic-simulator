using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadEdit : MonoBehaviour {
    public GameObject start;
    public GameObject startDirection;
    public GameObject end;
    public GameObject endDirection;
    public Camera mainCamera;

    private GameObject currentlyPulling = null;
    public Bezier bezier;
    private FlatBezierRenderer pathRenderer;
    public Material highlight, noHighlight;

    void Start() {
    }

    void Update() {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Input.GetAxis("Fire1") != 0.0f) {
            if (currentlyPulling == null && Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, 1 << 7)) {
                currentlyPulling = hit.transform.gameObject;
                currentlyPulling.GetComponent<MeshRenderer>().material = highlight;
            }
        } else {
            if (currentlyPulling != null) {
                currentlyPulling.GetComponent<MeshRenderer>().material = noHighlight;
            }
            currentlyPulling = null;
        }
        if (currentlyPulling != null) {
            Physics.Raycast(ray, out RaycastHit hit_, Mathf.Infinity, 1 << 6);
            Vector3 position = new Vector3(hit_.point.x, 0.0f, hit_.point.z);
            currentlyPulling.transform.position = position;
        }
    }
}
