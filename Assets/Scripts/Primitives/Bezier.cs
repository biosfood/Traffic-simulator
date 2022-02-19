using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Bezier {
    public Vector3 A, B, C, D;
    public float length;
    private static float perscision = 16f;
    public float[] lengths = new float[(int) perscision + 1];

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

    public void updateLength() {
        length = 0f;
        Vector3 previous = A;
        lengths[0] = 0f;
        for (int i = 1; i <= perscision; i++) {
            Vector3 current = getPosition(i / perscision);
            length += (current - previous).magnitude;
            previous = current;
            lengths[i] = length;
        }
    }

    public float getT(float distance) {
        int index = 0;
        while (!(lengths[index] <= distance && lengths[index+1] > distance) && index < perscision-1) {
            index++;
        }
        float lengthBefore = lengths[index];
        float lengthAfter =  lengths[index+1];
        float segmentLength = lengthAfter - lengthBefore;
        float segmentFraction = (distance - lengthBefore) / segmentLength;
        return (index + segmentFraction) / perscision;
    }
    
    public Vector3 getPosition(float t) {
        float T = 1.0f - t;
        return
            A * (  T*T*T) +
            B * (3*T*T*t) +
            C * (3*T*t*t) +
            D * (  t*t*t);
    }

    public Vector3 getDirection(float t) {
        float T = 1.0f - t;
        return
            A * (-3*T*T        ) +
            B * ( 3*T*T - 6*T*t) +
            C * (-3*t*t + 6*T*t) +
            D * ( 3*t*t        );
    }

    public Vector3 getAcceleration(float t) {
        return
            A * (-6 *t + 6 ) +
            B * ( 6*t) +
            C * ( 18*t - 12 ) +
            D * (-18 *t + 6);
    }

    public float getRadius(float t) {
        Vector3 direction = getDirection(t);
        Vector3 acceleration = getAcceleration(t);
        return
            Mathf.Pow(direction.x * direction.x + direction.z * direction.z, 1.5f) / 
            Mathf.Abs(direction.x * acceleration.z - direction.z * acceleration.x);
    }
}
