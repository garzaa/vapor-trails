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
        points[0] = start.position;
        points[1] = end.position;
        line.SetPositions(points);
    }
}