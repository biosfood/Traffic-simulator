using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Node {
    private FlatCircleRenderer circle = new FlatCircleRenderer(0.2f, 0.1f, 32);
    private FlatCircleRenderer fullCircle = new FlatCircleRenderer(0f, 1f, 32);
    
    public Vector3 position;
    public List<Road> roads = new List<Road>();
    public GameObject gameObject, circleObject, roadObject, textObject;
    public Vector3 direction;
    public Config config;
    private Transform parent;
    public NodeData nodeData;

    public Node(Vector3 position, Transform parent, Config config) {
        this.position = position;
        this.parent = parent;
        this.config = config;
    }

    public Node init<T>() where T: NodeData {
        gameObject = new GameObject();
        gameObject.transform.position = position;
        gameObject.transform.parent = parent;
        gameObject.AddComponent<SphereCollider>().radius = 1f;
        gameObject.layer = 7;
        nodeData = gameObject.AddComponent<T>();
        nodeData.node = this;
        nodeData.config = config;

        circleObject = new GameObject();
        circleObject.AddComponent<MeshRenderer>().material = config.roadEditMaterial;
        circleObject.AddComponent<MeshFilter>().mesh = circle.mesh;
        circleObject.transform.parent = gameObject.transform;
        circleObject.transform.localPosition = new Vector3(0f, 0.01f, 0f);
        
        roadObject = new GameObject();
        roadObject.AddComponent<MeshRenderer>().material = config.roadMaterial;
        roadObject.AddComponent<MeshFilter>().mesh = fullCircle.mesh;
        roadObject.transform.parent = gameObject.transform;
        roadObject.transform.localPosition = new Vector3(0f, 0f, 0f);

        textObject = new GameObject();
        textObject.AddComponent<MeshRenderer>();
        TextMesh text = textObject.AddComponent<TextMesh>();
        text.text = "";
        text.anchor = TextAnchor.MiddleCenter;
        textObject.transform.localScale = Vector3.one * 0.25f;
        textObject.transform.parent = gameObject.transform;
        textObject.transform.localPosition = new Vector3(0f, 3f, 0f);
        update();
        return this;
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

    abstract public void pull(Vector3 position);

    private void updateDirection() {
        List<Road> incomingRoads = roads.FindAll(it => it.nodes[0] == this);
        List<Road> outgoingRoads = roads.FindAll(it => it.nodes[1] == this);
        Vector3  inDirection = getAverageDirection(incomingRoads, 0, outgoingRoads);
        Vector3 outDirection = getAverageDirection(outgoingRoads, 1, incomingRoads);
        direction = (inDirection - outDirection) * 0.25f;
    }

    public void lateUpdate(Road caller) {
        updateDirection();
        foreach (Road road in roads) {
            if (!road.Equals(caller)) {
                road.update(false);
            }
        }
    }

    public void update() {
        updateDirection();
        foreach (Road road in roads) {
            road.update(true);
        }
    }

    private Vector3 getAverageDirection(List<Road> roads, int index, List<Road> otherRoads) {
        Vector3 sum = new Vector3(0f, 0f, 0f);
        if (roads.Count == 0) {
            return sum;
        }
        foreach (Road road in roads) {
            Vector3 direction = road.nodes[1-index].position - position;
            sum += direction;
            direction.Normalize();
            foreach (Road otherRoad in otherRoads) {
                Vector3 otherDirection = otherRoad.nodes[index].position - position;
                otherDirection.Normalize();
                if ((direction + otherDirection).magnitude < 0.1f) {
                    return direction * (otherRoad.nodes[index].position - road.nodes[1-index].position).magnitude / 4f;
                }
            }
        }
        return sum / roads.Count;
    }

    public abstract void delete();

    public void forceDelete() {
        foreach (Road road in roads) {
            road.nodes.Remove(this);
            road.delete();
        }
        GameObject.Destroy(gameObject);
    }
}
