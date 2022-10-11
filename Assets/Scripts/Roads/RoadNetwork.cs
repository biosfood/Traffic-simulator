using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class RoadNetwork {
    public List<Node> nodes = new List<Node>();
    public List<Road> roads = new List<Road>();
}
