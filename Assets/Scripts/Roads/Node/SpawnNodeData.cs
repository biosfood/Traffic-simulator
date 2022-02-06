using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnNodeData : MonoBehaviour {
    private SpawnNode node;
    public Config config;
    public ExitNodeData[] targets = new ExitNodeData[0];
    
    void Start() {
        node = new SpawnNode(transform.position, transform, config);
    }

    void Update() {
    }
}
