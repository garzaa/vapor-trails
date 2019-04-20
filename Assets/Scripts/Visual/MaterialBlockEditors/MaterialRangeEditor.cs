using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialRangeEditor : MaterialBlockEditor
{
    [Range(0, 1)]
    public float value;
    float valueLastFrame;

    void Update() {
        if (value != valueLastFrame) {
            GetBlock();
            block.SetFloat(valueName, value);
            SetBlock();
        }
        valueLastFrame = value;
    }
}
