using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlatCircleRenderer {
    public Mesh mesh = new Mesh();
    public float radius, width;
    private int segments;

    public FlatCircleRenderer(float radius, float width, int segments) {
        this.radius = radius;
        this.width = width;
        this.segments = segments;
        update();
    }

    public void update() {
        Vector3[] vertices = new Vector3[2*segments+6];
        int[] indices = new int[6*segments];
        for (int i = 0; i < segments; i++) {
            float alpha = (float) i / segments * 2 * Mathf.PI;
            indices[6*i  ] = 2*i  ;
            indices[6*i+3] = 2*i  ;
            indices[6*i+5] = 2*i+1;
            if (i == 0) {
                indices[6*i+1] = 2*segments-2;
                indices[6*i+2] = 2*segments-1;
                indices[6*i+4] = 2*segments-1;
            } else {
                indices[6*i+1] = 2*i-2;
                indices[6*i+2] = 2*i-1;
                indices[6*i+4] = 2*i-1;
            }
            Vector3 direction = new Vector3(Mathf.Sin(alpha), 0.0f, Mathf.Cos(alpha));
            vertices[2*i  ] = direction * radius;
            vertices[2*i+1] = direction * (radius + width);
        }
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = indices;
    }
}
