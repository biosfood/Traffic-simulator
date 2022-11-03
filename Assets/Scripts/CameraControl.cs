using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Runtime.InteropServices;

public class CameraControl : MonoBehaviour {
    [DllImport("user32.dll")]
    static extern bool SetCursorPos(int X, int Y);

    public float mouseSensitivity = 1.0f, movementSpeed = 0.1f;
    private float pitch = 10.0f, yaw = 0.0f, scale = 0.0f, scrollSpeed = 2.0f;
    public bool photoMode = false;

    public Camera mainCamera;
    public GameObject roads;
    private static Vector3 photoModeEuler = new Vector3(90, 0, 0);

    void Start() {
    }

    void Update() {
        bool moving = Input.GetAxis("Fire3") != 0.0f;
        Cursor.visible = !moving;
        if (moving) {
            float dx = Input.GetAxis("Mouse X") * mouseSensitivity;
            float dy = Input.GetAxis("Mouse Y") * mouseSensitivity;
            if (Input.GetAxis("Jump") != 0.0f) {
                Vector3 direction = new Vector3(mainCamera.transform.forward.x, 0.0f, mainCamera.transform.forward.z);
                direction.Normalize();
                Vector3 normal = new Vector3(direction.z, 0.0f, -direction.x);
                transform.position -= direction * movementSpeed * dy * transform.localScale.x;
                transform.position -= normal    * movementSpeed * dx * transform.localScale.x;
            } else {
                pitch -= dy;
                yaw += dx;
                pitch = Mathf.Clamp(pitch, 10, 80);
            }
        }
        if (photoMode) {
            transform.eulerAngles = photoModeEuler;
        } else {
            transform.eulerAngles = new Vector3(pitch, yaw, 0);
        }
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0.0f) {
            scale += scroll * (-scrollSpeed);
            scale = Mathf.Clamp(scale, -2, 4);
            float realScale = Mathf.Pow(2, scale);
            transform.localScale = new Vector3(realScale, realScale, realScale);
        }
    }
}
