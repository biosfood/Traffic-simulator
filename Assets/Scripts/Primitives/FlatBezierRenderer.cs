using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlatBezierRenderer {
    private int percision;
    private float width;
    public Bezier bezier;
    public Mesh mesh = new Mesh();
    public FlatBezierRenderer(Bezier bezier, int percision, float width) {
        this.percision = percision;
        this.width = width;
        this.bezier = bezier;
        update();
    }

    public void update() {
        float realWidth = width / 2;
        int vertexCount = 6 * (percision+1);
        Vector3[] vertices = new Vector3[vertexCount / 3];
        int[] indices = new int[vertexCount];
        Vector3 position = bezier.getPosition(0.0f);
        Vector3 tangent = getTangent(0.0f);
        vertices[0] = position + tangent * realWidth;
        vertices[1] = position - tangent * realWidth;
        for (int i = 1; i <= percision; i++) {
            float t = (float) i / percision;
            indices[6*i  ] = 2*i  ;
            indices[6*i+1] = 2*i-1;
            indices[6*i+2] = 2*i-2;
            indices[6*i+3] = 2*i+1;
            indices[6*i+4] = 2*i-1;
            indices[6*i+5] = 2*i  ;
            
            position = bezier.getPosition(t);
            tangent = getTangent(t);
            vertices[2*i  ] = position + tangent * realWidth;
            vertices[2*i+1] = position - tangent * realWidth;
        }
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = indices;
        mesh.Optimize();
    }

    private Vector3 getTangent(float t) {
        Vector3 direction = bezier.getDirection(t);
        direction.Normalize();
        return new Vector3(-direction.z, 0, direction.x);
    }
}
