using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialColorEditor : MaterialBlockEditor {
    public Color color;
    Color valueLastFrame;

    void Update() {
        if (color != valueLastFrame) {
            material.SetColor(valueName, color);
        }
        valueLastFrame = color;
    }
}