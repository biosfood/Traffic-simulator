using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using UnityEditor;
using System.IO;
using System.Text;

public class PhotoMode : MonoBehaviour, IPointerDownHandler {
    public Config config;

    public void OnPointerDown(PointerEventData eventData) {
        config.cameraControl.photoMode = !config.cameraControl.photoMode;
    }
}
