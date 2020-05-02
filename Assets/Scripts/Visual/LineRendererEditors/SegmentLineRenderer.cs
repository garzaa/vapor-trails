using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class SegmentLineRenderer : LineRendererEditor {
    public List<Transform> positions;
    Vector3[] points;

    override protected void Start() {
        base.Start();
        line.useWorldSpace = false;
    }

    void Update() {
        SetPoints();
    }

    void LateUpdate() {
        SetPoints();
    }

    void SetPoints() {
        // don't reallocate the array every frame
        if (points.Length != positions.Count) {
            points = new Vector3[positions.Count];
            line.positionCount = positions.Count;
        }
        line.SetPositions(points);
    }

}