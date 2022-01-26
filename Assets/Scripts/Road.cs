using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Road {
    public Node start, end;
    FlatBezierRenderer bezierRenderer;
    Material material;
    
    public Road(Node start, Node end, Material material) {
        this.start = start;
        this.end = end;
        this.material = material;
        bezierRenderer = new FlatBezierRenderer(new Bezier(), 50, 0.2f);
    }
        

    public void initialize(Transform parent) {
        GameObject child = new GameObject();
        child.transform.position = Vector3.zero;
        child.AddComponent<MeshRenderer>().material = material;
        child.AddComponent<MeshFilter>().mesh = bezierRenderer.mesh;
        child.transform.parent = parent;
        update();
    }

    public void update() {
        bezierRenderer.bezier.A = start.position;
        bezierRenderer.bezier.B = 0.5f * (start.position + end.position);
        bezierRenderer.bezier.C = 0.5f * (end.position + start.position);
        bezierRenderer.bezier.D = end.position;
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
        return 
            (otherRoad.start.Equals(start)) && (otherRoad.end.Equals(end)) || 
            (otherRoad.start.Equals(end)) && (otherRoad.end.Equals(start));
    }

    public override int GetHashCode() {
        return start.GetHashCode() << 16 | end.GetHashCode();
    }
}
