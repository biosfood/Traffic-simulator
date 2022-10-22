using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using UnityEditor;
using System.IO;
using System.Text;

public class SaveTravelTimes : MonoBehaviour, IPointerDownHandler {
    public Config config;

    [Serializable]
    private struct Times {
        public List<float> travelTimes;
    }

    public static void saveTravelTimesToFile(string filename, Config config) {
        Times time = new Times();
        time.travelTimes = config.travelTimes;
        string jsonData = JsonUtility.ToJson(time);
        File.WriteAllBytes(filename, Encoding.ASCII.GetBytes(jsonData));
    }

    public void OnPointerDown(PointerEventData eventData) {
        #if UNITY_EDITOR
        string filePath = EditorUtility.SaveFilePanel("Save travel times", "", 
                                                      "times.json", "json");
        if (filePath.Length == 0) {
            return;
        }
        saveTravelTimesToFile(filePath, config);
        #endif
    }
}
