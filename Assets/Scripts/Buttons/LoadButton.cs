using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using UnityEditor;
using System.IO;
using System.Text;


public class LoadButton : MonoBehaviour, IPointerDownHandler {
    public Config config;

    public void OnPointerDown(PointerEventData eventData) {
        config.onClick();
        string filePath = EditorUtility.OpenFilePanel("Open a road network", "", "json");
        if (filePath.Length == 0) {
            return;
        }
        config.roadNetwork.clear();
        string fileContent = File.ReadAllText(filePath);
        SaveStruct saveData = JsonUtility.FromJson<SaveStruct>(fileContent);
        // todo: put the saved nodes and nodes into the world
        print(saveData.nodes.Count);
    }
}
