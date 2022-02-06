using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Road {
    public List<Node> nodes = new List<Node>();
    Bezier path, arrow1, arrow2;
    private FlatBezierRenderer pathLine, roadBody, arrow1Renderer, arrow2Renderer;
    private Config config;
    private MeshCollider collider;
    public GameObject gameObject;

    public Road(Node start, Node end, Config config) {
        nodes.Add(start);
        nodes.Add(end);
        this.config = config;
    }
        

    public void initialize(Transform parent) {
        path = new Bezier();
        pathLine = new FlatBezierRenderer(path, 50, 0.05f);
        roadBody = new FlatBezierRenderer(path, 50, 2f);

        arrow1 = new Bezier();
        arrow1Renderer = new FlatBezierRenderer(arrow1, 3, 0.05f);

        arrow2 = new Bezier();
        arrow2Renderer = new FlatBezierRenderer(arrow2, 3, 0.05f);

        gameObject = new GameObject();
        gameObject.transform.parent = parent;
        gameObject.transform.position = Vector3.zero;

        GameObject lineChild = new GameObject();
        lineChild.transform.position = Vector3.zero;
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
        update(true);
    }

    private void setupArrow(FlatBezierRenderer renderer, Transform parent) {
        GameObject child = new GameObject();
        child.transform.position = Vector3.zero;
        child.AddComponent<MeshRenderer>().material = config.roadEditMaterial;
        child.AddComponent<MeshFilter>().mesh = renderer.mesh;
        child.transform.parent = parent;
    }

    public void update(bool updateOthers) {
        path.A = nodes[0].position;
        path.D = nodes[1].position;
        if (nodes[0].roads.Count == 2) {
            path.B = nodes[0].position + 
                0.25f * (nodes[1].position - nodes[0].getOther(nodes[1]).position);
        } else {
            path.B = 0.5f * (nodes[0].position + nodes[1].position);
        }
        if (nodes[1].roads.Count == 2) {
            path.C = nodes[1].position + 
                0.25f * (nodes[0].position - nodes[1].getOther(nodes[0]).position);
        } else {
            path.C = 0.5f * (nodes[0].position + nodes[1].position);
        }
        if (updateOthers) {
            foreach (Node node in nodes) {
                node.lateUpdate(this);
            }
        }
        pathLine.update();
        roadBody.update();
        collider.sharedMesh = roadBody.mesh;

        Vector3 midPosition = path.getPosition(0.5f);
        Vector3 midDirection = path.getDirection(0.5f);
        midDirection.Normalize();
        Vector3 midNormal = new Vector3(-midDirection.z, 0, midDirection.x);
        midNormal.Normalize();
        arrow1.A = midPosition;
        arrow2.A = midPosition;
        arrow1.B = midPosition - midDirection * 0.5f + midNormal * 0.5f;
        arrow2.B = arrow1.B - midNormal;
        arrow1.C = midPosition;
        arrow2.C = midPosition;
        arrow1.D = arrow1.B;
        arrow2.D = arrow2.B;
        arrow1Renderer.update();
        arrow2Renderer.update();
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
