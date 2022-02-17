using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NewtonPolynom {
    private List<float> coefficients = new List<float>();
    private List<float> xValues = new List<float>(), yValues = new List<float>();

    public NewtonPolynom() {
    }

    public void add(float x, float y) {
        xValues.Add(x);
        yValues.Add(y);
        coefficients.Add(getCoefficient(coefficients.Count, 0));
    }

    private float getCoefficient(int x, int y) {
        if (x == 0) {
            return yValues[y];
        }
        return (getCoefficient(x-1, y+1) - getCoefficient(x-1, y)) / (xValues[x+y] - xValues[y]);
    }

    public float evaluate(float x) {
        float current = 0;
        for (int i = 0; i < coefficients.Count; i++) {
            int index = coefficients.Count-1-i;
            current = coefficients[index] + (x - xValues[index]) * current;
        }
        return current;
    }
}
