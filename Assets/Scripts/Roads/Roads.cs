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
        pullingNode = new CustomNode(groundPosition, transform, config).init<NodeData>();
    }

    private void endRoad(Ray ray, Vector3 groundPosition) {
        drawMode = DrawMode.None;
        Node endNode = null;
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, 1 << 7)) {
            endNode = hit.transform.gameObject.GetComponent<NodeData>().node;
        } else {
            endNode = new CustomNode(groundPosition, transform, config).init<NodeData>();
        }
        Road road = new Road(pullingNode, endNode, config);
        if (pullingNode.roads.Contains(road)) {
            Road other = pullingNode.roads.Find(it => road.Equals(it));
            if (other.nodes[0] != road.nodes[0]) {
                other.nodes = road.nodes;
                other.update(true);
            }
        } else if (pullingNode != endNode) {
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

    private void handleTrafficLightEditing(Ray ray) {
        RaycastHit hit;
        if (!Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 7)) {
            return;
        }
        NodeData nodeData = hit.transform.gameObject.GetComponent<NodeData>();
        if (nodeData == null) {
            return;
        }
        Node node = nodeData.node;
        if (!(node is CustomNode)) {
            return;
        }
        CustomNode customNode = (CustomNode) node;
        if (Input.GetAxis("Fire1") != 0.0f) {
            if (drawMode == DrawMode.DragRoad) {
                return;
            }
            drawMode = DrawMode.DragRoad;
            customNode.lightPhase++;
        } else if (Input.GetAxis("Fire2") != 0.0f) {
            if (drawMode == DrawMode.DragRoad) {
                return;
            }
            customNode.lightPhase--;
            drawMode = DrawMode.DragRoad;
        } else {
            drawMode = DrawMode.None;
        }
    }

    public Vector3 snapGroundPosition(Vector3 position, float snapStrength, int interval) {
        float x = position.x, z = position.z;
        float xRounded = Mathf.Round(x / interval) * interval;
        float zRounded = Mathf.Round(z / interval) * interval;
        if (Mathf.Abs(x - xRounded) < snapStrength) {
            x = xRounded;
        }
        if (Mathf.Abs(z - zRounded) < snapStrength) {
            z = zRounded;
        }
        return new Vector3(x, 0.0f, z);
    }

    void Update() {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 6);
        Vector3 groundPosition = snapGroundPosition(hit.point, .5f, 2);

        if (config.mode == Mode.DrawRoad) {
            handleRoadDrawing(ray, groundPosition);
        } else if (config.mode == Mode.DeleteRoad) {
            handleRoadRemoving(ray);
        }
        if (config.mode == Mode.TrafficLight) {
            handleTrafficLightEditing(ray);
            return;
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
