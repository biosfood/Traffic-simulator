using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomNode : Node {
    public CustomNode(Vector3 position, Transform parent, Config config): base(position, parent, config) {
    }

    override public void delete() {
        foreach (Road road in roads) {
            road.nodes.Remove(this);
            road.delete();
        }
        GameObject.Destroy(gameObject);
    }
}
