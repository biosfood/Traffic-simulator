using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarData : MonoBehaviour {
    public Car car;

    void Start() {
    }

    public void Update() {
        car.step(Time.deltaTime);
        transform.position = car.getPosition();
    }
}
