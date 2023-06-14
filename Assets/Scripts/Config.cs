using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;
using System;

public class Config : MonoBehaviour {
    public Material roadMaterial, roadEditMaterial, carBrakingMaterial, carAccelerationMaterial;
    public Mode mode = Mode.DeleteRoad;
    public Mode futureMode;
    public RectTransform highlight;
    public float carAirResistanceModifier = 0.3f, carFrontalArea = 4f, airDensity = 1.225f, carMass = 1000f;
    public Mesh carMesh;
    public Material carMaterial;
    public float power = 2500f;
    public List<float> travelTimes = new List<float>();
    public RoadNetwork roadNetwork = new RoadNetwork();
    public TrafficLights trafficLights;
    public CameraControl cameraControl;
    public float frequency, frequencyVariance;

    private string roadNetworkFileName, outputFileName;
    private float time;
    public Transform roads;
    public bool spawnCars = true;
    public bool giveWay = true;
       
    private static string getCLIArgumentValue(string name) {
        string[] arguments = System.Environment.GetCommandLineArgs();
        for (int i = 0; i < arguments.Length - 1; i++) {
            if (arguments[i] == name) {
                return arguments[i + 1];
            }
        }
        return "";
    }

    public void click(Mode mode, int index) {
        onClick();
        futureMode = mode;
        highlight.anchoredPosition = new Vector3(36f * (1.5f*index+1), 36f, 0f);
    }

    public void onClick() {
        futureMode = mode;
        mode = Mode.ClickButton;
    }

    private void Start() {
        roadNetworkFileName = getCLIArgumentValue("-i");
        outputFileName = getCLIArgumentValue("-o");

        if (roadNetworkFileName.Length > 0) {
            LoadButton.loadRoadNetworkFromFile(roadNetworkFileName, this);
        }
        string timeString = getCLIArgumentValue("-t");
        if (timeString.Length > 0) {
            time = float.Parse(timeString, CultureInfo.InvariantCulture.NumberFormat);
        } else {
            time = float.PositiveInfinity;
        }
        string frequencyString = getCLIArgumentValue("-f");
        if (frequencyString.Length > 0) {
            frequency = float.Parse(frequencyString, CultureInfo.InvariantCulture.NumberFormat);
        }
        string redTimeString = getCLIArgumentValue("-red");
        if (redTimeString.Length > 0) {
            trafficLights.redTime = float.Parse(redTimeString, CultureInfo.InvariantCulture.NumberFormat);
        }
        string greenTimeString = getCLIArgumentValue("-green");
        if (greenTimeString.Length > 0) {
            trafficLights.greenTime = float.Parse(greenTimeString, CultureInfo.InvariantCulture.NumberFormat);
        }
        string invertLightsString = getCLIArgumentValue("-invert");
        if (invertLightsString.Length > 0) {
            trafficLights.invert = bool.Parse(invertLightsString);
        }
    }

    private void Update() {
        if (Input.GetAxis("Fire1") == 0.0f && mode == Mode.ClickButton) {
            mode = futureMode;
        }
        time -= Time.deltaTime;
        if (time <= 0) {
            if (outputFileName.Length > 0) {
                SaveTravelTimes.saveTravelTimesToFile(outputFileName, this);
            }
            Application.Quit();
        }
    }
}
