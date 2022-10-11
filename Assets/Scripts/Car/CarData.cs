using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarData : MonoBehaviour {
    public Car car;
    private float lifetime = 0f;

    void Start() {
    }

    public void Update() {
        car.step(Time.deltaTime);
        lifetime += Time.deltaTime;
        if (car.isAlive)  {
            transform.position = car.position;
            transform.forward = car.direction;
        } else {
            car.config.totalTravelTime += lifetime;
            car.config.totalCars++;
        }
    }
    
    public void printf(string data) {
        print(transform.position.ToString() + "  " + data); 
    }
}
