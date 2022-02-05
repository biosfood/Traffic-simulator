using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DrawMode {
    None,
    DrawRoad,
    DragRoad,
    DeleteRoad
}

public class Roads : MonoBehaviour {
    public Camera mainCamera;
    public Material material;
    public Config config;
    public DrawMode drawMode = DrawMode.None;

    void Start() {
    }

    private Node pullingNode = null;

    private void startRoad(Ray ray, Vector3 groundPosition) {
        drawMode = DrawMode.DrawRoad;
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, 1 << 7)) {
            pullingNode = hit.transform.gameObject.GetComponent<NodeData>().node;
            return;
        }
        pullingNode = new CustomNode(groundPosition, transform, config);
    }

    private void endRoad(Ray ray, Vector3 groundPosition) {
        drawMode = DrawMode.None;
        Node endNode = null;
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, 1 << 7)) {
            endNode = hit.transform.gameObject.GetComponent<NodeData>().node;
        } else {
            endNode = new CustomNode(groundPosition, transform, config);
        }
        Road road = new Road(pullingNode, endNode, config);
        if (! pullingNode.roads.Contains(road) && pullingNode != endNode) {
            pullingNode.roads.Add(road);
            endNode.roads.Add(road);
            road.initialize(transform);
        }
        pullingNode = null;
    }

    private void handleRoadDrawing(Ray ray, Vector3 groundPosition) {
        if (Input.GetAxis("Fire1") == 1.0f) {
            if (drawMode == DrawMode.None) {
                startRoad(ray, groundPosition);
            }
        } else if (drawMode == DrawMode.DrawRoad) {
            endRoad(ray, groundPosition);
        }
    }

    private void handleRoadRemoving(Ray ray) {
        if (drawMode == DrawMode.None && Input.GetAxis("Fire1") != 0.0f) {
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 7)) {
                hit.transform.gameObject.GetComponent<NodeData>().node.delete();
                drawMode = DrawMode.DeleteRoad;
            } else if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 8)) {
                hit.transform.gameObject.GetComponent<RoadData>().road.delete();
                drawMode = DrawMode.DeleteRoad;
            }
        } else if (drawMode == DrawMode.DeleteRoad && Input.GetAxis("Fire1") == 0.0f) {
            drawMode = DrawMode.None;
        }
    }

    void Update() {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 6);
        Vector3 groundPosition = new Vector3(hit.point.x, 0.0f, hit.point.z);

        if (config.mode == Mode.DrawRoad) {
            handleRoadDrawing(ray, groundPosition);
        } else if (config.mode == Mode.DeleteRoad) {
            handleRoadRemoving(ray);
        }
        if (Input.GetAxis("Fire2") != 0.0f) {
            if (drawMode == DrawMode.None && Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 7)) {
                drawMode = DrawMode.DragRoad;
                pullingNode = hit.transform.gameObject.GetComponent<NodeData>().node;
            }
        } else if (drawMode == DrawMode.DragRoad) {
            drawMode = DrawMode.None;
        }
        if (drawMode == DrawMode.DragRoad) {
            pullingNode.pull(groundPosition);
        }
    }
}
