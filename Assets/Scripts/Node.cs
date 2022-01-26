using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node {
    public FlatCircleRenderer circle = new FlatCircleRenderer(0.2f, 0.05f, 32);
    public Vector3 position;
    public List<Road> roads = new List<Road>();
    public GameObject gameObject = new GameObject();

    public Node(Vector3 position, Transform parent, Material material) {
        gameObject.transform.position = position;
        gameObject.AddComponent<MeshRenderer>().material = material;
        gameObject.AddComponent<MeshFilter>().mesh = circle.mesh;
        gameObject.AddComponent<SphereCollider>().radius = 0.25f;
        gameObject.transform.parent = parent;
        gameObject.layer = 7;
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

    public void pull(Vector3 position) {
        this.position = position;
        gameObject.transform.position = position;
        foreach (Road road in roads) {
            road.update();
        }
    }
}
