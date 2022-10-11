using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnNodeData : NodeData {
    public Config config;
    public ExitNodeData[] targets;
    public float timeLeft = 0f, interval = 5f;

    override public void Start() {
        node = new SpawnNode(transform.position, transform, config);
        base.Start();
    }

    void Update() {
        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0f) {
            timeLeft = interval;
            ExitNodeData target = targets[Random.Range(0, targets.Length)];
            Route route = new Route(node, target.node);
            if (route.isValid && route.roads[0].carsOnRoute.Count == 0) {
                new Car(route, transform, config);
            }
        }
    }
}
