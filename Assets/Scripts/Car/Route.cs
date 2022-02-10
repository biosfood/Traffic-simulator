using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Route {
    public Node start, end;
    public List<Road> roads = new List<Road>();
    public bool isValid = false;

    public Route(Node start, Node end) {
        this.start = start;
        this.end = end;
        if (start.roads.Count == 0) {
            return;
        }
        Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node>();
        cameFrom[start] = null;
        Dictionary<Node, float> travelDistance = new Dictionary<Node, float>();
        travelDistance.Add(start, 0f);
        Dictionary<Node, float> totalDistance = new Dictionary<Node, float>();
        totalDistance.Add(start, (start.position-end.position).magnitude);
        HashSet<Node> openNodes = new HashSet<Node>();
        openNodes.Add(start);

        while (openNodes.Count > 0) {
            Node currentNode = null;
            float currentMinTotalDistance = float.PositiveInfinity;
            foreach (Node node in openNodes) {
                float distance = totalDistance[node];
                if (distance < currentMinTotalDistance) {
                    currentMinTotalDistance = distance;
                    currentNode = node;
                }
            }
            if (currentNode == end) {
                while (cameFrom[currentNode] != null) {
                    roads.Add(currentNode.roads.Find(it => it.nodes.Contains(cameFrom[currentNode])));
                    currentNode = cameFrom[currentNode];
                }
                roads.Reverse();
                isValid = true;
                return;
            }
            openNodes.Remove(currentNode);
            foreach (Road road in currentNode.roads) {
                if (road.nodes[0] != currentNode) {
                    continue;
                }
                Node neighbour = road.nodes[1];
                float neighbourTravelDistance = totalDistance[currentNode] + (currentNode.position - neighbour.position).magnitude;
                if ((!travelDistance.ContainsKey(neighbour)) || neighbourTravelDistance < travelDistance[neighbour]) {
                    cameFrom[neighbour] = currentNode;
                    travelDistance[neighbour] = neighbourTravelDistance;
                    totalDistance[neighbour] = neighbourTravelDistance + (neighbour.position - end.position).magnitude;
                    if (!openNodes.Contains(neighbour)) {
                        openNodes.Add(neighbour);
                    }
                }
            }
        }
    }
}

