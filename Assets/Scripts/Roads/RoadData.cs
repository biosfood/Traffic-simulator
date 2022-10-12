using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadData : MonoBehaviour {
    public Road road;

    public void Start() {
        road.config.roadNetwork.roads.Add(road);
    }

    public void OnDestroy() {
        road.config.roadNetwork.roads.Remove(road);
    }
}
