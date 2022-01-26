using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Roads : MonoBehaviour {
    public List<Node> nodes = new List<Node>();
    public List<Road> roads = new List<Road>();
    public FlatBezierRenderer roadRenderer;
    public Camera mainCamera;
    private bool down = false;
    public Material material;

    void Start() {
    }

    private Node startNode;

    private void startRoad() {
        Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, Mathf.Infinity, 1 << 6);
        foreach (Node node in nodes) {
            if ((hit.point - node.position).magnitude < 1.0f) {
                startNode = node;
                down = true;
                return;
            }
        } 
        Vector3 position = new Vector3(hit.point.x, 0.0f, hit.point.z);
        startNode = new Node(position, transform, material);
        nodes.Add(startNode);
        down = true;
    }

    private void endRoad() {
        Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, Mathf.Infinity, 1 << 6);
        Node endNode = null;
        foreach (Node node in nodes) {
            if ((hit.point - node.position).magnitude < 1.0f) {
                endNode = node;
                break;
            }
        }
        if (endNode == null) {
            Vector3 position = new Vector3(hit.point.x, 0.0f, hit.point.z);
            endNode = new Node(position, transform, material);
            nodes.Add(endNode);
        }
        
        Road road = new Road(startNode, endNode, material);
        if (!roads.Contains(road)) {
            road.initialize(transform);
            roads.Add(road);
            startNode.roads.Add(road);
            endNode.roads.Add(road);
        }
        startNode = null;
        down = false;
    }

    void Update() {
        if (Input.GetAxis("Fire1") != 0.0f) {
            if (!down) {
                startRoad();
            }
        } else {
            if (down) {
                endRoad();
            }
        }
    }
}
