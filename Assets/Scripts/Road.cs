using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Road {
    public List<Node> nodes = new List<Node>();
    Bezier path;
    FlatBezierRenderer pathLine, roadBody;
    Config config;
    MeshCollider collider;

    public Road(Node start, Node end, Config config) {
        nodes.Add(start);
        nodes.Add(end);
        path = new Bezier();
        pathLine = new FlatBezierRenderer(path, 50, 0.05f);
        roadBody = new FlatBezierRenderer(path, 50, 2f);
        this.config = config;
    }
        

    public void initialize(Transform parent) {
        GameObject child = new GameObject();
        child.transform.parent = parent;
        GameObject lineChild = new GameObject();
        lineChild.transform.position = Vector3.zero;
        lineChild.AddComponent<MeshRenderer>().material = config.roadEditMaterial;
        lineChild.AddComponent<MeshFilter>().mesh = pathLine.mesh;
        lineChild.transform.parent = child.transform;

        GameObject roadChild = new GameObject();
        collider = roadChild.AddComponent<MeshCollider>();
        roadChild.transform.position = Vector3.zero;
        roadChild.AddComponent<MeshRenderer>().material = config.roadMaterial;
        roadChild.transform.parent = child.transform;
        roadChild.layer = 8;
        roadChild.AddComponent<MeshFilter>().mesh = roadBody.mesh;
        roadChild.AddComponent<RoadData>().road = this;
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
}
