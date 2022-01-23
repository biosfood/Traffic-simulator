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

    public CameraControl cameraControl;

    private FlatBezierRenderer roadRenderer;

    public Material highlight, noHighlight;

    void Start() {
        roadRenderer = new FlatBezierRenderer(cameraControl.road, 100, 0.1f);
        gameObject.AddComponent<MeshFilter>().mesh = roadRenderer.mesh;
    }

    void updateRoad(Vector3 position) {
        if (currentlyPulling == start) {
            cameraControl.road.A.x = position.x;
            cameraControl.road.A.z = position.z;
        } else if (currentlyPulling == startDirection) {
            cameraControl.road.B.x = position.x;
            cameraControl.road.B.z = position.z;
        } else if (currentlyPulling == endDirection) {
            cameraControl.road.C.x = position.x;
            cameraControl.road.C.z = position.z;
        } else {
            cameraControl.road.D.x = position.x;
            cameraControl.road.D.z = position.z;
        }
        cameraControl.roadRenderer.update();
        roadRenderer.update();
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
            updateRoad(position);
        }
    }
}
