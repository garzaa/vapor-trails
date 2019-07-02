using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class StreakRenderer : LineRendererEditor {
    public Transform start;
    public Transform end;

    override protected void Start() {
        base.Start();
        line.positionCount = 2;
    }

    void LateUpdate() {
        Vector3[] points = new Vector3[2];
        points[0] = start.localPosition;
        points[1] = end.localPosition;
        line.SetPositions(points);
    }
}