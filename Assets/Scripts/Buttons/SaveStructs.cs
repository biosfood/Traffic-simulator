using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct SaveNode {
    public Vector3 position;
    public string type;
    public List<int> targets;
    public int lightPhase;
}

[Serializable]
public struct SaveRoad {
    public int start, end;
}

[Serializable]
public struct SaveStruct {
    public List<SaveNode> nodes;
    public List<SaveRoad> roads;
}
