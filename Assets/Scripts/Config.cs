using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Config : MonoBehaviour {
    public Material roadMaterial, roadEditMaterial, carMaterial;
    public Mode mode = Mode.DeleteRoad;
    public Mode futureMode;
    public RectTransform highlight;

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
