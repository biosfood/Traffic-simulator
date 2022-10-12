using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class SaveButton : MonoBehaviour, IPointerDownHandler {
    public Config config;

    [Serializable]
    private struct SaveNode {
        public Vector3 position;
        public string type;
    }

    [Serializable]
    private struct SaveRoad {
        public int start, end;
    }

    [Serializable]
    private struct SaveStruct {
        public List<SaveNode> nodes;
        public List<SaveRoad> roads;
    }

    public void OnPointerDown(PointerEventData eventData) {
        config.onClick();
        List<SaveNode> nodes = new List<SaveNode>();
        foreach (Node node in config.roadNetwork.nodes) {
            SaveNode saveNode = new SaveNode();
            saveNode.position = node.position;
            if (node is SpawnNode) {
                saveNode.type = "spawn";
            } else if (node is ExitNode) {
                saveNode.type = "exit";
            } else {
                saveNode.type = "";
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
        print(JsonUtility.ToJson(save));
    }
}
