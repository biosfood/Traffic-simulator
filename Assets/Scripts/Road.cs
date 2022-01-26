using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Road {
    public List<Node> nodes = new List<Node>();
    FlatBezierRenderer bezierRenderer;
    Material material;
    
    public Road(Node start, Node end, Material material) {
        nodes.Add(start);
        nodes.Add(end);
        this.material = material;
        bezierRenderer = new FlatBezierRenderer(new Bezier(), 50, 0.2f);
    }
        

    public void initialize(Transform parent) {
        GameObject child = new GameObject();
        child.transform.position = Vector3.zero;
        child.AddComponent<MeshRenderer>().material = material;
        child.AddComponent<MeshFilter>().mesh = bezierRenderer.mesh;
        child.transform.parent = parent;
        update(true);
    }

    public void update(bool updateOthers) {
        bezierRenderer.bezier.A = nodes[0].position;
        if (nodes[0].roads.Count == 2) {
            if (updateOthers) {
                nodes[0].lateUpdate(this);
            }
            bezierRenderer.bezier.B = nodes[0].position + 
                0.25f * (nodes[1].position - nodes[0].getOther(nodes[1]).position);
        } else {
            bezierRenderer.bezier.B = 0.5f * (nodes[0].position + nodes[1].position);
        }
        if (nodes[1].roads.Count == 2) {
            if (updateOthers) {
                nodes[1].lateUpdate(this);
            }
            bezierRenderer.bezier.C = nodes[1].position + 
                0.25f * (nodes[0].position - nodes[1].getOther(nodes[0]).position);
        } else {
            bezierRenderer.bezier.C = 0.5f * (nodes[0].position + nodes[1].position);
        }
        bezierRenderer.bezier.D = nodes[1].position;
        bezierRenderer.update();
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
