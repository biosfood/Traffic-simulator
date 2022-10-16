using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car {
    private Route route;
    public Road road;
    private int roadIndex = 0;
    public float roadPositon = 0f, speed = 0f, airResistance;
    private GameObject gameObject;
    public Config config;
    public Vector3 position, direction;
    public bool isAlive = true;
    private CarData carData;
    private float brakingTime, brakingDistance;
    private static float g = - Physics.gravity.y;
    private static float breakawayAcceleration = 0.9f * g, rollingAcceleration = 0.02f * g;

    public Car(Route route, Transform parent, Config config) {
        this.route = route;
        this.config = config;
        road = route.roads[0];
        foreach (Road currentRoad in route.roads) {
            currentRoad.carsOnRoute.Add(this);
        }
        airResistance = config.carAirResistanceModifier * config.carFrontalArea * config.airDensity * 0.5f;
        setupGameObject(parent);
        position = road.path.getPosition(0f);
    }

    private void setupGameObject(Transform parent) {
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
    }

    private void updateBrakingParameters() {
        float B = - airResistance;
        float A = - rollingAcceleration - breakawayAcceleration;
        float sqrtAB = Mathf.Sqrt(A * B);
        float sqrtAbyB = Mathf.Sqrt(A / B);
        float offset = Mathf.Atan(1f / sqrtAbyB * speed);
        brakingTime = offset;
        brakingDistance = ((float)(Mathf.Log((float)System.Math.Cosh(sqrtAB * brakingTime - offset)) - 
                                   Mathf.Log((float)System.Math.Cosh(offset)))) / B;
    }

    private void incrementRoad() {
        road.carsOnRoute.Remove(this);
        roadPositon -= road.path.length;
        roadIndex++;
        if (roadIndex == route.roads.Count) {
            GameObject.Destroy(gameObject);
            road.carsOnRoute.Remove(this);
            isAlive = false;
            return;
        }
        road = route.roads[roadIndex];
    }

    private float getMaxSpeed(Road road, float distance) {
        return Mathf.Sqrt((float)(Mathf.Abs(road.path.getRadius(road.path.getT(distance)))) * breakawayAcceleration);
    }

    private bool needsBraking(float totalDistance, float maxSpeed) {
        float B = - airResistance;
        float A = - rollingAcceleration - breakawayAcceleration;
        float sqrtAB = Mathf.Sqrt(A * B);
        float sqrtAbyB = Mathf.Sqrt(A / B);
        float sqrtBbyA = 1 / sqrtAbyB;
        float offset = Mathf.Atan(sqrtBbyA * speed);
        float time = (offset - Mathf.Atan(sqrtBbyA * maxSpeed)) / sqrtAB;
        float distance = (-Mathf.Log(Mathf.Cos(sqrtAB * time - offset)) + Mathf.Log(Mathf.Cos(offset))) / B;
        return totalDistance <= distance;
    }

    private bool needsBrakingForCar(Car car) {
        if (roadIndex == route.roads.Count - 1) {
            return false;
        }
        Road conflict = car.road;
        float otherDistance = conflict.path.length - car.roadPositon;
        for (int i = car.roadIndex; !this.route.roads.Contains(conflict) && i < car.route.roads.Count;) {
            i++;
            conflict = car.route.roads[i];
            otherDistance += conflict.path.length;
        }
        otherDistance -= conflict.path.length;
        float thisDistance = - this.roadPositon;
        for (int i = this.roadIndex ; i < this.route.roads.Count && this.route.roads[i] != conflict; i++) {
            thisDistance += this.route.roads[i].path.length;
        }
        float time = Mathf.Max(0.5f, thisDistance / speed);
        float otherTraveledDistance = car.speed * time;
        float currentCarDistance = thisDistance - otherDistance;
        float projectedDistance = otherTraveledDistance - otherDistance;
        if (currentCarDistance < 0 || currentCarDistance > car.brakingDistance + 3f) {
            return false;
        }
        if (currentCarDistance > 0 && currentCarDistance < 3) {
            Debug.DrawLine(position + 1.5f * Vector3.up, car.position + 2 * Vector3.up, Color.blue, 0f, false);
            return true;
        }
        if (projectedDistance > 0 && projectedDistance < 4) {
            Debug.DrawLine(position + 1.5f * Vector3.up, car.position + 2 * Vector3.up, Color.yellow, 0f, false);
            return true;
        }
        if (needsBraking(Mathf.Min(projectedDistance, currentCarDistance) - 3f, car.speed)) {
            Debug.DrawLine(position + 1.5f * Vector3.up, car.position + 2 * Vector3.up, Color.red, 0f, false);
            return true;
        }
        return false;
    }

    private bool isBraking() {
        float stoppingDistance = Mathf.Max(brakingDistance, 1f);
        Road currentRoad = road;
        float currentRoadPosition = roadPositon;
        int currentRoadIndex = roadIndex;
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
            }
            if (needsBraking(totalDistance, getMaxSpeed(currentRoad, currentRoadPosition))) {
                return true;
            }
        }
    end:
        List<Car> carsToCheck = new List<Car>();
        for (int i = this.roadIndex; i < this.route.roads.Count; i++) {
            foreach (Car car in this.route.roads[i].carsOnRoute) {
                if (!carsToCheck.Contains(car)) {
                    carsToCheck.Add(car);
                }
            }
        }
        foreach (Car car in carsToCheck) {
            if (car != this && needsBrakingForCar(car)) {
                return true;
            }
        }
        return false;
    }

    private static float atanh(float x) {
        return 0.5f * (Mathf.Log(1 + x) - Mathf.Log(1 - x));
    }

    private void applyAcceleration(float A, float B, float deltaT) {
        if (A * B > 0) {
            if (speed == 0) {
                return;
            }
            float sqrtAB = Mathf.Sqrt(A * B);
            float sqrtAbyB = Mathf.Sqrt(A / B);
            float offset = Mathf.Atan(1f / sqrtAbyB * speed);
            speed = - sqrtAbyB * Mathf.Tan(sqrtAB * deltaT - offset);
            roadPositon += (-Mathf.Log(Mathf.Cos(sqrtAB * deltaT - offset)) + Mathf.Log(Mathf.Cos(offset))) / B;
        } else {
            float sqrtAB = Mathf.Sqrt(-A * B);
            float sqrtAbyB = Mathf.Sqrt(-A / B);
            float offset = atanh(1.0f / sqrtAbyB * speed);
            speed = sqrtAbyB * (float) System.Math.Tanh(sqrtAB * deltaT + offset);
            roadPositon += ((float)(Mathf.Log((float)System.Math.Cosh(sqrtAB * deltaT - offset)) - Mathf.Log((float)System.Math.Cosh(offset)))) / B;
        }
    }

    public void step(float deltaTime) {
        airResistance = config.carAirResistanceModifier * config.carFrontalArea * config.airDensity * 0.5f;
        updateBrakingParameters();
        float B = - airResistance;
        float A = - rollingAcceleration;
        if (isBraking()) {
            gameObject.GetComponent<MeshRenderer>().material = config.carBrakingMaterial;
            A -= breakawayAcceleration;
        } else {
            gameObject.GetComponent<MeshRenderer>().material = config.carAccelerationMaterial;
            if (speed > 0) {
                A += Mathf.Min(config.power / speed, breakawayAcceleration);
            } else {
                A += 1f;
            }
        }
        applyAcceleration(A, B, deltaTime);
        speed = Mathf.Max(0, speed);
        while (roadPositon > road.path.length) {
            incrementRoad();
        }
        float t = road.path.getT(roadPositon);
        position = road.path.getPosition(t);
        direction = road.path.getDirection(t);
        direction.Normalize();
    }
}
