using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Road {
    public List<Node> nodes = new List<Node>();
    public Bezier path;
    private Bezier arrow1, arrow2;
    private FlatBezierRenderer pathLine, roadBody, arrow1Renderer, arrow2Renderer;
    private Config config;
    private MeshCollider collider;
    public GameObject gameObject;
    public List<Car> carsOnRoute = new List<Car>();

    public Road(Node start, Node end, Config config) {
        nodes.Add(start);
        nodes.Add(end);
        this.config = config;
    }
        

    public void initialize(Transform parent) {
        path = new Bezier();
        pathLine = new FlatBezierRenderer(path, 32, 0.05f);
        roadBody = new FlatBezierRenderer(path, 32, 2f);

        arrow1 = new Bezier();
        arrow1Renderer = new FlatBezierRenderer(arrow1, 3, 0.05f);

        arrow2 = new Bezier();
        arrow2Renderer = new FlatBezierRenderer(arrow2, 3, 0.05f);

        gameObject = new GameObject();
        gameObject.transform.parent = parent;
        gameObject.transform.position = Vector3.zero;

        GameObject lineChild = new GameObject();
        lineChild.transform.position = new Vector3(0f, 0.01f, 0f);
        lineChild.AddComponent<MeshRenderer>().material = config.roadEditMaterial;
        lineChild.AddComponent<MeshFilter>().mesh = pathLine.mesh;
        lineChild.transform.parent = gameObject.transform;

        GameObject bodyChild = new GameObject();
        bodyChild.AddComponent<MeshRenderer>().material = config.roadMaterial;
        bodyChild.AddComponent<MeshFilter>().mesh = roadBody.mesh;
        bodyChild.AddComponent<RoadData>().road = this;
        collider = bodyChild.AddComponent<MeshCollider>();
        bodyChild.transform.parent = gameObject.transform;
        bodyChild.layer = 8;

        setupArrow(arrow1Renderer, gameObject.transform);
        setupArrow(arrow2Renderer, gameObject.transform);

        foreach (Node node in nodes) {
            node.update();
        }
    }

    private void setupArrow(FlatBezierRenderer renderer, Transform parent) {
        GameObject child = new GameObject();
        child.transform.position = new Vector3(0f, 0.01f, 0f);
        child.AddComponent<MeshRenderer>().material = config.roadEditMaterial;
        child.AddComponent<MeshFilter>().mesh = renderer.mesh;
        child.transform.parent = parent;
    }

    public void update(bool updateOthers) {
        path.A = nodes[0].position;
        path.B = nodes[0].position + nodes[0].direction;
        path.C = nodes[1].position - nodes[1].direction;
        path.D = nodes[1].position;
        if (updateOthers) {
            foreach (Node node in nodes) {
                node.lateUpdate(this);
            }
        }
        collider.sharedMesh = roadBody.mesh;

        Vector3 midPosition = path.getPosition(0.5f);
        Vector3 midDirection = path.getDirection(0.5f);
        midDirection.Normalize();
        Vector3 midNormal = new Vector3(-midDirection.z, 0, midDirection.x);
        midNormal.Normalize();
        updateArrow(arrow1, midPosition, midDirection, midNormal, 1.0f,  arrow1Renderer);
        updateArrow(arrow2, midPosition, midDirection, midNormal, -1.0f, arrow2Renderer);
        pathLine.update();
        roadBody.update();
        path.updateLength();
    }

    private void updateArrow(Bezier arrow, Vector3 position, Vector3 direction, Vector3 normal, float offset, FlatBezierRenderer renderer) {
        arrow.A = position;
        arrow.B = position - direction * 0.5f + offset * normal * 0.5f;
        arrow.C = position;
        arrow.D = arrow.B;
        renderer.update();
    }

    override public bool Equals(object other) {
        if (other == null || !(other is Road)) {
            return false;
        }
        if (other == this) {
            return true;
        }
        Road otherRoad = (Road) other;
        foreach (Node node in nodes) {
            if (! otherRoad.nodes.Contains(node)) {
                return false;
            }
        }
        return true;
    }

    public override int GetHashCode() {
        return nodes[0].GetHashCode() << 16 | nodes[1].GetHashCode();
    }

    public void delete() {
        foreach (Node node in nodes) {
            node.roads.Remove(this);
            if (node.roads.Count == 0) {
                node.delete();
            } else {
                node.update();
            }
        }
        GameObject.Destroy(gameObject);
    }
}
