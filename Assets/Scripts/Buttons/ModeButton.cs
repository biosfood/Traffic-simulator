using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ModeButton : MonoBehaviour, IPointerDownHandler {
    public Config config;
    public Mode mode;
    public int index;

    public void OnPointerDown(PointerEventData eventData) {
        config.click(mode, index);
    }
}
