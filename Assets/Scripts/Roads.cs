using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Roads : MonoBehaviour {
    public List<Node> nodes = new List<Node>();
    public List<Road> roads = new List<Road>();
    public FlatBezierRenderer roadRenderer;
    public Camera mainCamera;
    private bool drawing = false;
    public Material material;

    void Start() {
    }

    private Node startNode;

    private void startRoad() {
        Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, Mathf.Infinity, 1 << 6);
        foreach (Node node in nodes) {
            if ((hit.point - node.position).magnitude < 1.0f) {
                startNode = node;
                drawing = true;
                return;
            }
        } 
        Vector3 position = new Vector3(hit.point.x, 0.0f, hit.point.z);
        startNode = new Node(position, transform, material);
        nodes.Add(startNode);
        drawing = true;
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
            startNode.roads.Add(road);
            endNode.roads.Add(road);
            roads.Add(road);
            road.initialize(transform);
        }
        startNode = null;
        drawing = false;
    }

    Node pulling = null;

    void Update() {
        if (Input.GetAxis("Fire1") != 0.0f) {
            if (!drawing) {
                startRoad();
            }
        } else if (drawing) {
            endRoad();
        }
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Input.GetAxis("Fire2") != 0.0f) {
            if (pulling == null && Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, 1 << 7)) {
                pulling = nodes.Find(node => node.gameObject == hit.transform.gameObject);
            }
        } else {
            pulling = null;
        }
        if (pulling != null) {
            Physics.Raycast(ray, out RaycastHit hit_, Mathf.Infinity, 1 << 6);
            Vector3 position = new Vector3(hit_.point.x, 0.0f, hit_.point.z);
            pulling.pull(position);
        }
    }
}
