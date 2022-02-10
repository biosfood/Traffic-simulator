using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnNodeData : MonoBehaviour {
    private SpawnNode node;
    public Config config;
    public ExitNodeData[] targets;
    public float timeLeft = 0f, interval = 5f;

    void Start() {
        node = new SpawnNode(transform.position, transform, config);
    }

    void Update() {
        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0f) {
            timeLeft = interval;
            ExitNodeData target = targets[Random.Range(0, targets.Length)];
            Route route = new Route(node, target.node);
            if (route.isValid) {
                new Car(route, transform, config);
            }
        }
    }
}
