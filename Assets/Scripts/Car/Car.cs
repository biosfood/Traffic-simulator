using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car {
    private Route route;
    public Road currentRoad;
    public float roadPositon = 0f, speed = 0f, brakingAcceleration, acceleration, airResistance, t;
    private int roadIndex = 0;
    private GameObject gameObject;
    private Config config;
    public Vector3 position, direction;
    public bool isAlive = true;

    public Car(Route route, Transform parent, Config config) {
        this.route = route;
        this.config = config;
        currentRoad = route.roads[0];
        currentRoad.cars.Add(this);

        gameObject = new GameObject();
        FlatCircleRenderer renderer = new FlatCircleRenderer(0.8f, 0.2f, 32, 0.01f);
        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.material = config.carAccelerationMaterial;
        meshRenderer.receiveShadows = false;
        gameObject.AddComponent<MeshFilter>().mesh = renderer.mesh;
        gameObject.transform.parent = parent;
        GameObject carMesh = new GameObject();
        carMesh.AddComponent<MeshRenderer>().material = config.carMaterial;
        carMesh.AddComponent<MeshFilter>().mesh = config.carMesh;
        carMesh.transform.parent = gameObject.transform;
        CarData carData = gameObject.AddComponent<CarData>();
        carData.car = this;
        position = currentRoad.path.getPosition(0f);
        carData.Update();
        brakingAcceleration = 0.2f * -Physics.gravity.y;
        acceleration = config.carTorque / (config.carWheelRadius * config.carMass);
        airResistance = config.carAirResistanceModifier*config.carFrontalArea*config.airDensity / 
                        (2*config.carMass);
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

    private float getMaxSpeed(Road road, float distance) {
        return Mathf.Sqrt((float) (-Mathf.Abs(road.path.getRadius(road.path.getT(distance))) / 
                                    0.2 * Physics.gravity.y));
    }

    private bool isBraking() {
        float stoppingDistance = 0.5f * speed * speed / brakingAcceleration;
        Road road = currentRoad;
        float position = roadPositon;
        int currentRoadIndex = roadIndex;
        for (int i = 1; i <= 10; i++) {
            position += stoppingDistance / 10;
            while (position >= road.path.length) {
                currentRoadIndex ++;
                if (currentRoadIndex == route.roads.Count) {
                    return false;
                }
                position -= road.path.length;
                road = route.roads[currentRoadIndex];
            }
            float deltaV = speed - getMaxSpeed(road, position);
            float distanceNeededToBrake = speed * speed / brakingAcceleration
                                         - 0.5f * deltaV * deltaV / brakingAcceleration;
            if (stoppingDistance / 10 * i < distanceNeededToBrake) {
                return true;
            }
        }
        return false;
    }
    
    public void step(float deltaTime) {
        speed -= airResistance * deltaTime * speed * speed;
        if (isBraking()) {
            speed -= brakingAcceleration * deltaTime;
            gameObject.GetComponent<MeshRenderer>().material = config.carBrakingMaterial;
        } else {
            speed += acceleration * deltaTime;
            gameObject.GetComponent<MeshRenderer>().material = config.carAccelerationMaterial;
        }
        roadPositon += deltaTime * speed;
        while (roadPositon > currentRoad.path.length) {
            incrementRoad();
        }
        t = currentRoad.path.getT(roadPositon);
        position = currentRoad.path.getPosition(t);
        direction = currentRoad.path.getDirection(t);
        direction.Normalize();
    }
}
