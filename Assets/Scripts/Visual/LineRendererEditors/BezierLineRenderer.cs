using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(LineRenderer))]
public class BezierLineRenderer : MonoBehaviour {
    public Transform point1;
    public Transform point2;
    public Transform point3;
    public LineRenderer lineRenderer;
    public int vertexCount = 12;

    void DrawBezier() {
        var pointList = new List<Vector3>();
        if (vertexCount < 2) {
            Debug.Log("brainlet alert");
            return;
        }
        for (float ratio = 0; ratio <= 1; ratio += 1.0f / vertexCount)
        {
            var tangentLineVertex1 = Vector3.Lerp(point1.position, point2.position, ratio);
            var tangentLineVertex2 = Vector3.Lerp(point2.position, point3.position, ratio);
            var bezierpoint = Vector3.Lerp(tangentLineVertex1, tangentLineVertex2, ratio);
            pointList.Add(bezierpoint);
        }
        lineRenderer.positionCount = pointList.Count;
        lineRenderer.SetPositions(pointList.ToArray());
    }

#if UNITY_EDITOR
    void Update() {
        DrawBezier();
    }
#endif

    // draw AFTER the parallax layers are updated
    void LateUpdate() {
        DrawBezier();
    }

}
