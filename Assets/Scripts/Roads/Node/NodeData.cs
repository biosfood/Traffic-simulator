using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeData : MonoBehaviour {
    public Node node =  null;

    public virtual void Start() {
        node.config.roadNetwork.nodes.Add(node);
    }

    void OnDestroy() {
        node.config.roadNetwork.nodes.Remove(node);
    }
}
