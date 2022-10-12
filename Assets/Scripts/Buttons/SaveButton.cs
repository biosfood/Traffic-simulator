using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using UnityEditor;
using System.IO;
using System.Text;

public class SaveButton : MonoBehaviour, IPointerDownHandler {
    public Config config;

    public void OnPointerDown(PointerEventData eventData) {
        config.onClick();
        List<SaveNode> nodes = new List<SaveNode>();
        foreach (Node node in config.roadNetwork.nodes) {
            SaveNode saveNode = new SaveNode();
            saveNode.position = node.position;
            if (node is SpawnNode) {
                saveNode.type = "spawn";
                List<int> targets = new List<int>();
                foreach (ExitNodeData exit in (node.nodeData as SpawnNodeData).targets) {
                    targets.Add(config.roadNetwork.nodes.IndexOf(exit.node));
                }
                saveNode.targets = targets;
            } else if (node is ExitNode) {
                saveNode.type = "exit";
                saveNode.targets = new List<int>();
            } else {
                saveNode.type = "";
                saveNode.targets = new List<int>();
            }
            nodes.Add(saveNode);
        }
        List<SaveRoad> roads = new List<SaveRoad>();
        foreach (Road road in config.roadNetwork.roads) {
            SaveRoad saveRoad = new SaveRoad();
            saveRoad.start = config.roadNetwork.nodes.IndexOf(road.nodes[0]);
            saveRoad.end = config.roadNetwork.nodes.IndexOf(road.nodes[1]);
            roads.Add(saveRoad);
        }
        SaveStruct save = new SaveStruct();
        save.nodes = nodes;
        save.roads = roads;
        string jsonData = JsonUtility.ToJson(save);
        string filePath = EditorUtility.SaveFilePanel("Save current road network", "", 
                                                      "untiteledIntersection.json", "json");
        if (filePath.Length == 0) {
            return;
        }
        File.WriteAllBytes(filePath, Encoding.ASCII.GetBytes(jsonData));
    }
}
