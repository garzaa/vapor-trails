using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
[ExecuteInEditMode]
public class LineWidthEditor : LineRendererEditor {
  
    public float startWidth;
    public float endWidth;

    void Update() {
        line.startWidth = startWidth;
        line.endWidth = endWidth;
    }
}
