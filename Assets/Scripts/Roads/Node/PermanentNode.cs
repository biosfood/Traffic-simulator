using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PermanentNode : Node {
    public PermanentNode(Vector3 position, Transform parent, Config config): base(position, parent, config) {
    }

    override public void delete() {
    }
}
