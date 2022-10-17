using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeData : MonoBehaviour {
    public Node node =  null;
    public Config config;

    public virtual void Start() {
        node.config.roadNetwork.nodes.Add(node);
    }

    void OnDestroy() {
        node.config.roadNetwork.nodes.Remove(node);
    }

    private void Update() {
        if (!(node is CustomNode)) {
            return;
        }
        CustomNode customNode = (CustomNode) node;
        if (customNode.lightPhase == 0) {
            return;
        }
        if (customNode.isPassable) {
            node.circleObject.GetComponent<MeshRenderer>().material = config.carAccelerationMaterial;
        } else {
            node.circleObject.GetComponent<MeshRenderer>().material = config.carBrakingMaterial;
        }
    }
}
