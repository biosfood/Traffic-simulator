using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bezier {
    public Vector3 A, B, C, D;
    public float length;

    public Bezier(Vector3 A, Vector3 B, Vector3 C, Vector3 D) {
        this.A = A;
        this.B = B;
        this.C = C;
        this.D = D;
        updateLength();
    }

    public Bezier() {
        this.A = Vector3.zero;
        this.B = Vector3.zero;
        this.C = Vector3.zero;
        this.D = Vector3.zero;
        updateLength();
    }
    
    public Vector3 getPosition(float t) {
        float T = 1.0f - t;
        return
            A * (  T*T*T) +
            B * (3*T*T*t) +
            C * (3*T*t*t) +
            D * (  t*t*t);
    }

    public void updateLength() {
        length = 0f;
        float steps = 50f;
        Vector3 previous = A;
        for (int i = 1; i <= steps; i++) {
            Vector3 current = getPosition(i / steps);
            length += (current - previous).magnitude;
            previous = current;
        }
    }

    public Vector3 getDirection(float t) {
        float T = 1.0f - t;
        return
            A * (-3*T*T        ) +
            B * ( 3*T*T - 6*T*t) +
            C * (-3*t*t + 6*T*t) +
            D * ( 3*t*t        );
    }
}
