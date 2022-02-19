using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car {
    private Route route;
    public Road currentRoad;
    public float roadPositon = 0f, speed = 0f;
    private int roadIndex = 0;
    private GameObject gameObject;
    private Config config;
    public Vector3 position;
    public bool isAlive = true;

    public Car(Route route, Transform parent, Config config) {
        this.route = route;
        this.config = config;
        currentRoad = route.roads[0];
        currentRoad.cars.Add(this);

        gameObject = new GameObject();
        FlatCircleRenderer renderer = new FlatCircleRenderer(0.8f, 0.2f, 32);
        gameObject.AddComponent<MeshRenderer>().material = config.carMaterial;
        gameObject.AddComponent<MeshFilter>().mesh = renderer.mesh;
        gameObject.transform.parent = parent;
        CarData carData = gameObject.AddComponent<CarData>();
        carData.car = this;
        position = currentRoad.path.getPosition(0f);
        carData.Update();
    }

    private void incrementRoad() {
        roadPositon -= currentRoad.path.length;
        roadIndex++;
        if (roadIndex == route.roads.Count) {
            GameObject.Destroy(gameObject);
            isAlive = false;
            return;
        }
        currentRoad.cars.Remove(this);
        currentRoad = route.roads[roadIndex];
        currentRoad.cars.Add(this);
    }
    
    public void step(float deltaTime) {
        speed += 
            -(deltaTime*config.carAirResistanceModifier*config.carFrontalArea*config.airDensity*speed*speed) / 
            (2*config.carMass) + 
            (config.carTorque*deltaTime) / (config.carWheelRadius * config.carMass)
            ;
        float t = currentRoad.path.getT(roadPositon);
        float maxSpeed = Mathf.Sqrt((float) (-Mathf.Abs(currentRoad.path.getRadius(t)) / 0.2 * Physics.gravity.y));
        speed = Mathf.Min(speed, maxSpeed);
        float distance = deltaTime * speed;
        roadPositon += distance;
        while (roadPositon > currentRoad.path.length) {
            incrementRoad();
        }
        position = currentRoad.path.getPosition(currentRoad.path.getT(roadPositon));
    }
}
