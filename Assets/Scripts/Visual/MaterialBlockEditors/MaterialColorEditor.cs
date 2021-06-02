using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MaterialColorEditor : MaterialBlockEditor {
    public Color color;
    Color valueLastFrame;

    override protected void Start() {
        base.Start();
        GetBlock();
        block.SetColor(valueName, color);
        SetBlock();
    }

    // only compile with the update loop in the editor for speed :^)
    #if UNITY_EDITOR
    void Update() {
        if (!Application.isPlaying) {
            LateUpdate();
        }
    }
    #endif

    void LateUpdate() {
        if (color != valueLastFrame) {
            GetBlock();
            block.SetColor(valueName, color);
            SetBlock();
        }
        valueLastFrame = color;
    }
}
