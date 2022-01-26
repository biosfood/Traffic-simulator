using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Runtime.InteropServices;

public class CameraControl : MonoBehaviour {
    [DllImport("user32.dll")]
    static extern bool SetCursorPos(int X, int Y);

    public float mouseSensitivity = 1.0f;
    private float pitch = 10.0f;
    private float yaw = 0.0f;
    private float scale = 0.0f;
    public float scrollSpeed = 2.0f;

    public float groundSize = 5.0f;

    public Camera mainCamera;
    public GameObject cursor;
    public GameObject roads;
    public Vector3 cursorPosition = new Vector3();

    void Start() {
    }

    void Update() {
        bool moving = Input.GetAxis("Fire3") != 0.0f;
        Cursor.visible = !moving;
        if (moving) {
            float dx = Input.GetAxis("Mouse X") * mouseSensitivity;
            float dy = Input.GetAxis("Mouse Y") * mouseSensitivity;
            pitch -= dy;
            yaw += dx;
            pitch = Mathf.Clamp(pitch, 10, 80);
            transform.eulerAngles = new Vector3(pitch, yaw, 0);
        }
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0.0f) {
            scale += scroll * (-scrollSpeed);
            scale = Mathf.Clamp(scale, -2, 2);
            float realScale = Mathf.Pow(2, scale);
            transform.localScale = new Vector3(realScale, realScale, realScale);
        }
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, 1 << 6)) {
            Vector3 position = new Vector3(
                Mathf.Clamp(hit.point.x, -groundSize, groundSize), 0.0f,
                Mathf.Clamp(hit.point.z, -groundSize, groundSize));
            cursor.transform.position = position;
            cursorPosition = position;
        }
    }
}
