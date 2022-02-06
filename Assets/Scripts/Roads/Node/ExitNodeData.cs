using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitNodeData: MonoBehaviour {
    private ExitNode node;
    public Config config;
    
    void Start() {
        node = new ExitNode(transform.position, transform, config);
    }

    void Update() {
    }
}
