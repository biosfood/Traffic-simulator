using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Config : MonoBehaviour {
    public Material roadMaterial, roadEditMaterial, carBrakingMaterial, carAccelerationMaterial;
    public Mode mode = Mode.DeleteRoad;
    public Mode futureMode;
    public RectTransform highlight;
    public float carAirResistanceModifier = 0.3f, carFrontalArea = 4f, airDensity = 1.225f, carMass = 1000f;
    public Mesh carMesh;
    public Material carMaterial;
    public float power = 100000f;
    public List<float> travelTimes = new List<float>();
    public RoadNetwork roadNetwork = new RoadNetwork();
    public TrafficLights trafficLights;
    public CameraControl cameraControl;

    public void click(Mode mode, int index) {
        onClick();
        futureMode = mode;
        highlight.anchoredPosition = new Vector3(36f * (1.5f*index+1), 36f, 0f);
    }

    public void onClick() {
        futureMode = mode;
        mode = Mode.ClickButton;
    }

    void Update() {
        if (Input.GetAxis("Fire1") == 0.0f && mode == Mode.ClickButton) {
            mode = futureMode;
        }
    }
}
