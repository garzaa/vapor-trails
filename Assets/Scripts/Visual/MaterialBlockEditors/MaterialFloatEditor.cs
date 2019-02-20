using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialFloatEditor : MaterialBlockEditor
{
    public float value;
    float valueLastFrame;

    void Update() {
        if (value != valueLastFrame) {
            material.SetFloat(valueName, value);
        }
        valueLastFrame = value;
    }
}
