using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node {
    public FlatCircleRenderer circle = new FlatCircleRenderer(0.2f, 0.05f, 32);
    public FlatCircleRenderer fullCircle = new FlatCircleRenderer(0f, 1f, 32);
    
    public Vector3 position;
    public List<Road> roads = new List<Road>();
    public GameObject gameObject;

    public Node(Vector3 position, Transform parent, Config config) {
        gameObject = new GameObject();
        gameObject.transform.position = position;
        gameObject.transform.parent = parent;
        gameObject.AddComponent<SphereCollider>().radius = 0.5f;
        gameObject.layer = 7;

        GameObject nodeCircle = new GameObject();
        nodeCircle.AddComponent<MeshRenderer>().material = config.roadEditMaterial;
        nodeCircle.AddComponent<MeshFilter>().mesh = circle.mesh;
        nodeCircle.transform.parent = gameObject.transform;
        nodeCircle.transform.localPosition = Vector3.zero;
        
        GameObject nodeRoad = new GameObject();
        nodeRoad.AddComponent<MeshRenderer>().material = config.roadMaterial;
        nodeRoad.AddComponent<MeshFilter>().mesh = fullCircle.mesh;
        nodeRoad.transform.parent = gameObject.transform;
        nodeRoad.transform.localPosition = Vector3.zero;
        this.position = position;
    }

    public Node getOther(Node caller) {
        if (roads.Count != 2) {
            return null;
        }
        foreach (Road road in roads) {
            if (! road.nodes.Contains(caller)) {
                return road.nodes.Find(test => test != this);
            }
        }
        return null;
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
            road.update(true);
        }
    }

    public void lateUpdate(Road caller) {
        foreach (Road road in roads) {
            if (!road.Equals(caller)) {
                road.update(false);
            }
        }
    }

    public void update() {
        foreach (Road road in roads) {
            road.update(true);
        }
    }
}
