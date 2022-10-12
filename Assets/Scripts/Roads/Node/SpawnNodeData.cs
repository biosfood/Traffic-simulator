using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnNodeData : NodeData {
    public List<ExitNodeData> targets = new List<ExitNodeData>();
    public float timeLeft = 0f, interval = 5f;

    void Update() {
        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0f && targets.Count > 0) {
            timeLeft = interval;
            ExitNodeData target = targets[Random.Range(0, targets.Count)];
            Route route = new Route(node, target.node);
            if (route.isValid && route.roads[0].carsOnRoute.Count == 0) {
                new Car(route, transform, config);
            }
        }
    }
}
