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
    public Config config;

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
        startNode = new Node(position, transform, config);
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
            endNode = new Node(position, transform, config);
            nodes.Add(endNode);
        }
        Road road = new Road(startNode, endNode, config);
        if (!roads.Contains(road) && startNode != endNode) {
            startNode.roads.Add(road);
            endNode.roads.Add(road);
            roads.Add(road);
            road.initialize(transform);
            road.update(true);
        }
        startNode = null;
        drawing = false;
    }

    Node pulling = null;

    void Update() {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (config.mode == Mode.DrawRoad) {
            if (Input.GetAxis("Fire1") != 0.0f) {
                if (!drawing) {
                    startRoad();
                }
            } else if (drawing) {
                endRoad();
            }
        } else if (config.mode == Mode.DeleteRoad) {
            if (!drawing && Input.GetAxis("Fire1") != 0.0f) {
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 8)) {
                    drawing = true;
                    Road road = hit.transform.gameObject.GetComponent<RoadData>().road;
                    roads.Remove(road);
                    foreach (Node node in road.nodes) {
                        node.roads.Remove(road);
                        node.update();
                    }
                    Destroy(hit.transform.parent.gameObject);
                }
            } else if (drawing && Input.GetAxis("Fire1") == 0.0f) {
                drawing = false;
            }
        }
        if (Input.GetAxis("Fire2") != 0.0f) {
            if (pulling == null && Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 7)) {
                pulling = nodes.Find(node => node.gameObject == hit.transform.gameObject);
            }
        } else {
            pulling = null;
        }
        if (pulling != null) {
            Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 6);
            Vector3 position = new Vector3(hit.point.x, 0.0f, hit.point.z);
            pulling.pull(position);
        }
    }
}
