using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarData : MonoBehaviour {
    public Car car;

    void Start() {
    }

    public void Update() {
        car.step(Time.deltaTime);
        if (car.isAlive)  {
            transform.position = car.position;
            transform.forward = car.direction;
        }
    }
}
