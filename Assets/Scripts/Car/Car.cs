using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car {
    private Route route;
    private Road currentRoad;
    private float currentPosition = 0f, speed = 0.25f;
    private int roadIndex = 0;
    GameObject gameObject;

    public Car(Route route, Transform parent, Config config) {
        this.route = route;
        currentRoad = route.roads[0];

        gameObject = new GameObject();
        FlatCircleRenderer renderer = new FlatCircleRenderer(0.8f, 0.2f, 32);
        gameObject.AddComponent<MeshRenderer>().material = config.carMaterial;
        gameObject.AddComponent<MeshFilter>().mesh = renderer.mesh;
        gameObject.transform.parent = parent;
        CarData carData = gameObject.AddComponent<CarData>();
        carData.car = this;
        carData.Update();
    }
    
    public Vector3 getPosition() {
        return currentRoad.path.getPosition(currentPosition);
    }

    public void step(float deltaTime) {
        currentPosition += deltaTime * speed;
        if (currentPosition > 1f) {
            currentPosition -= 1f;
            roadIndex++;
            if (roadIndex > route.roads.Count-1) {
                GameObject.Destroy(gameObject);
                return;
            }
            currentRoad = route.roads[roadIndex];
        }
    }
}
