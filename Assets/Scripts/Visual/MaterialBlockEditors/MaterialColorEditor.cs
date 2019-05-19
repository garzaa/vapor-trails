using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialColorEditor : MaterialBlockEditor {
    public Color color;
    Color valueLastFrame;

    override protected void Start() {
        base.Start();
        GetBlock();
        block.SetColor(valueName, color);
        SetBlock();
    }

    void Update() {
        if (color != valueLastFrame) {
            GetBlock();
            block.SetColor(valueName, color);
            SetBlock();
        }
        valueLastFrame = color;
    }
}