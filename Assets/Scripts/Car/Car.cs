using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car {
    private Route route;
    public Road road;
    public float roadPositon = 0f, speed = 0f, brakingAcceleration, acceleration, airResistance, t;
    private int roadIndex = 0;
    private GameObject gameObject;
    private Config config;
    public Vector3 position, direction;
    public bool isAlive = true;
    private CarData carData;

    public Car(Route route, Transform parent, Config config) {
        this.route = route;
        this.config = config;
        road = route.roads[0];
        foreach (Road currentRoad in route.roads) {
            currentRoad.carsOnRoute.Add(this);
        }

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
        carData = gameObject.AddComponent<CarData>();
        carData.car = this;
        position = road.path.getPosition(0f);
        carData.Update();
        brakingAcceleration = 0.2f * -Physics.gravity.y;
        acceleration = config.carTorque / (config.carWheelRadius * config.carMass);
        airResistance = config.carAirResistanceModifier * config.carFrontalArea * config.airDensity /
                        (2 * config.carMass);
    }

    private void incrementRoad() {
        roadPositon -= road.path.length;
        roadIndex++;
        road.carsOnRoute.Remove(this);
        if (roadIndex == route.roads.Count) {
            GameObject.Destroy(gameObject);
            isAlive = false;
            return;
        }
        road = route.roads[roadIndex];
    }

    private float getMaxSpeed(Road road, float distance) {
        return Mathf.Sqrt((float)(-Mathf.Abs(road.path.getRadius(road.path.getT(distance))) /
                                    0.2 * Physics.gravity.y));
    }

    private bool needsBraking(float totalDistance, float maxSpeed) {
        if (totalDistance < 0) {
            return true;
        }
        float deltaV = speed - maxSpeed;
        float distanceNeededToBrake = speed * speed / brakingAcceleration
                                     - 0.5f * deltaV * deltaV / brakingAcceleration;
        return totalDistance <= distanceNeededToBrake;
    }

    private float getDistance(Car car) {
        if (car.road == this.road) {
            //return this.roadPositon - car.roadPositon;
        }
        Road commonRoad = car.road;
        float otherPosition = car.roadPositon;
        for (int i = car.roadIndex; !this.route.roads.Contains(commonRoad) || 
            this.roadIndex > this.route.roads.IndexOf(commonRoad); i++) {
            commonRoad = car.route.roads[i];
            otherPosition -= commonRoad.path.length;
        }
        float ownPosition = this.roadPositon;
        for (int i = this.roadIndex ; this.route.roads[i] != commonRoad; i++) {
            ownPosition -= this.route.roads[i].path.length;
        }
        return otherPosition - ownPosition;
    }

    private bool needsBrakingForCar(Car car) {
        if (car.road == road && roadPositon > car.roadPositon) {
            return false;
        }
        float distance = getDistance(car);
        if (distance <= 0) {
            return false;
        }
        if (distance <= 3) {
            return true;
        }
        return needsBraking(distance-3, car.speed);
    }

    private bool isBraking() {
        float stoppingDistance = 0.5f * speed * speed / brakingAcceleration;
        stoppingDistance = Mathf.Min(5f, stoppingDistance);
        Road currentRoad = road;
        float currentRoadPosition = roadPositon;
        int currentRoadIndex = roadIndex;
        List<Car> carsToCheck = new List<Car>();
        foreach (Car car in currentRoad.carsOnRoute) {
            carsToCheck.Add(car);
        }
        int steps = 30;
        for (int i = 1; i <= steps; i++) {
            currentRoadPosition += stoppingDistance / steps;
            float totalDistance = stoppingDistance / steps * i;
            while (currentRoadPosition >= currentRoad.path.length) {
                currentRoadIndex++;
                if (currentRoadIndex == route.roads.Count) {
                    goto end;
                }
                currentRoadPosition -= currentRoad.path.length;
                currentRoad = route.roads[currentRoadIndex];
                foreach (Car car in currentRoad.carsOnRoute) {
                    if (!carsToCheck.Contains(car)) {
                        carsToCheck.Add(car);
                    }
                }
            }
            if (needsBraking(totalDistance, getMaxSpeed(currentRoad, currentRoadPosition))) {
                return true;
            }
        }
    end:
        foreach (Car car in carsToCheck) {
            if (car != this && needsBrakingForCar(car)) {
                return true;
            }
        }
        return false;
    }

    public void step(float deltaTime) {
        speed -= airResistance * deltaTime * speed * speed;
        if (isBraking()) {
            speed -= brakingAcceleration * deltaTime;
            speed = Mathf.Max(0, speed);
            gameObject.GetComponent<MeshRenderer>().material = config.carBrakingMaterial;
        } else {
            speed += acceleration * deltaTime;
            gameObject.GetComponent<MeshRenderer>().material = config.carAccelerationMaterial;
        }
        roadPositon += deltaTime * speed;
        while (roadPositon > road.path.length) {
            incrementRoad();
        }
        t = road.path.getT(roadPositon);
        position = road.path.getPosition(t);
        direction = road.path.getDirection(t);
        direction.Normalize();
    }
}
