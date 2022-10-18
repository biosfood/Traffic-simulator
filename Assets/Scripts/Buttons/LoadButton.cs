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
    public Transform roads;

    public void OnPointerDown(PointerEventData eventData) {
        config.onClick();
        string filePath = EditorUtility.OpenFilePanel("Open a road network", "", "json");
        if (filePath.Length == 0) {
            return;
        }
        config.roadNetwork.clear();
        string fileContent = File.ReadAllText(filePath);
        SaveStruct saveData = JsonUtility.FromJson<SaveStruct>(fileContent);
        List<Node> nodes = new List<Node>();
        List<(SpawnNodeData, List<int>)> spawnNodeData = new List<(SpawnNodeData, List<int>)>();
        foreach (SaveNode saveNode in saveData.nodes) {
            Node node = null;
            if (saveNode.type == "spawn") {
                node = new SpawnNode(saveNode.position, roads, config).init<SpawnNodeData>();
                spawnNodeData.Add((node.nodeData as SpawnNodeData, saveNode.targets));
            } else if (saveNode.type == "exit") {
                node = new ExitNode(saveNode.position, roads, config).init<ExitNodeData>();
            } else {
                CustomNode customNode = new CustomNode(saveNode.position, roads, config);
                customNode.lightPhase = saveNode.lightPhase;
                if (customNode.lightPhase != 0) {
                    customNode.isPassable = false;
                }
                node = customNode.init<NodeData>();
            }
            nodes.Add(node);
        }
        foreach ((SpawnNodeData, List<int>) pair in spawnNodeData) {
            SpawnNodeData nodeData = pair.Item1;
            List<int> targets = pair.Item2;
            foreach (int target in targets) {
                nodeData.targets.Add(nodes[target].nodeData as ExitNodeData);
            }
        }
        foreach (SaveRoad saveRoad in saveData.roads) {
            Road road = new Road(nodes[saveRoad.start], nodes[saveRoad.end], config);
            foreach (Node node in road.nodes) {
                node.roads.Add(road);
            }
            road.initialize(roads);
        }
    }
}
