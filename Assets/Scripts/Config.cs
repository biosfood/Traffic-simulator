using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Config : MonoBehaviour {
    public Material roadMaterial, roadEditMaterial, carBrakingMaterial, carAccelerationMaterial;
    public Mode mode = Mode.DeleteRoad;
    public Mode futureMode;
    public RectTransform highlight;
    public float carAirResistanceModifier = 0.3f, carFrontalArea = 4f, airDensity = 1.225f, carMass = 1000f;
    public float carTorque = 300f, carWheelRadius = 0.25f;
    public Mesh carMesh;
    public Material carMaterial;
    public float power = 1000f;
    public float totalTravelTime = 0f;
    public int totalCars = 0;

    public void click(Mode mode, int index) {
        this.mode = Mode.ClickButton;
        futureMode = mode;
        highlight.anchoredPosition = new Vector3(36f * (1.5f*index+1), 36f, 0f);
    }

    void Update() {
        if (Input.GetAxis("Fire1") == 0.0f && mode == Mode.ClickButton) {
            mode = futureMode;
        }
    }
}
