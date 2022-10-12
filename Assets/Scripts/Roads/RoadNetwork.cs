using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class RoadNetwork {
    public List<Node> nodes = new List<Node>();
    public List<Road> roads = new List<Road>();

    public void clear() {
        foreach (Road road in roads) {
            road.delete();
        }
        roads.Clear();
        foreach (Node node in nodes) {
            node.forceDelete();
        }
        nodes.Clear();
    }
}
