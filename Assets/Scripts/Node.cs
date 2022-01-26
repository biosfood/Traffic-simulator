using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node {
    public FlatCircleRenderer circle = new FlatCircleRenderer(0.4f, 0.1f, 50);
    public Vector3 position;
    public List<Road> roads = new List<Road>();

    public Node(Vector3 position, Transform parent, Material material) {
        GameObject child = new GameObject();
        child.transform.position = position;
        child.AddComponent<MeshRenderer>().material = material;
        child.AddComponent<MeshFilter>().mesh = circle.mesh;
        child.transform.parent = parent;
        this.position = position;
    }

    override public int GetHashCode() {
        return position.GetHashCode();
    }

    override public bool Equals(object other) {
        if ((other == null) || ! this.GetType().Equals(other.GetType())) {
            return false;
        }
        if (other == this) {
            return true;
        }
        return ((Node)other).position.Equals(position);
    }
}
