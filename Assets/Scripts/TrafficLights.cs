using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TrafficLightMode {
    Green,
    Red,
}

public class TrafficLights : MonoBehaviour {
    public Config config;
    public float time;
    public TrafficLightMode mode = TrafficLightMode.Green;
    public float greenTime = 10f;
    public float redTime = 2f;

    private void handleTurnGreen(List<CustomNode> nodes) {
        foreach (CustomNode node in nodes) {
            // todo: select only correct nodes
            node.isPassable = true;
        }

    }

    private void handleTurnRed(List<CustomNode> nodes) {
        foreach (CustomNode node in nodes) {
            node.isPassable = false;
        }
    }

    void Update() {
        time -= Time.deltaTime;
        if (time <= 0f) {
            List<CustomNode> nodes = new List<CustomNode>();
            foreach (Node node in config.roadNetwork.nodes) {
                if (node is CustomNode) {
                    CustomNode customNode = (CustomNode) node;
                    if (customNode.lightPhase != 0) {
                        nodes.Add(customNode);
                    }
                }
            }
            if (mode == TrafficLightMode.Green) {
                mode = TrafficLightMode.Red;
                time += redTime;
                handleTurnRed(nodes);
            } else {
                mode = TrafficLightMode.Green;
                time += greenTime;
                handleTurnGreen(nodes);
            }
        }
    }
}
