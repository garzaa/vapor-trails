using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
[ExecuteInEditMode]
public class LineColorEditor : LineRendererEditor {
  
    public Color startColor;
    public Color endColor;

   void Update() {
        line.startColor = startColor;
        line.endColor = endColor;
    }
}
