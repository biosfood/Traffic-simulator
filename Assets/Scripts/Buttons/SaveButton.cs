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
    private struct SaveStruct {
        public List<SaveNode> nodes;
    }

    public void OnPointerDown(PointerEventData eventData) {
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
        SaveStruct save = new SaveStruct();
        save.nodes = nodes;
        print(JsonUtility.ToJson(save));
    }
}
