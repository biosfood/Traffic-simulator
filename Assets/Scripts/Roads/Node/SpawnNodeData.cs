using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnNodeData : NodeData {
    public List<ExitNodeData> targets = new List<ExitNodeData>();
    public float timeLeft = 0f;

    void Update() {
        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0f && targets.Count > 0) {
            timeLeft = Random.Range(1 / (config.frequency * (1 - config.frequencyVariance)), 1 / (config.frequency * (1 + config.frequencyVariance)));
            ExitNodeData target = targets[Random.Range(0, targets.Count)];
            Route route = new Route(node, target.node);
            bool canSummon = route.isValid && config.spawnCars;
            foreach (Car car in route.roads[0].carsOnRoute) {
                if (car.roadPositon <= 3f) {
                    canSummon = false;
                }
                if (!canSummon) {
                    break;
                }
            }
            if (canSummon) {
                new Car(route, transform, config);
            } else {
                node.config.travelTimes.Add(0f);
            }
        }
    }
}
