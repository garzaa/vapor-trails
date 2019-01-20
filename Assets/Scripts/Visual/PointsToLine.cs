using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(LineRenderer))]
public class PointsToLine : MonoBehaviour {

	public List<Transform> points;
	LineRenderer lineRenderer;

	void Start() {
		lineRenderer = GetComponent<LineRenderer>();
		lineRenderer.positionCount = points.Count;
		lineRenderer.useWorldSpace = false;
	}

	void Update() {
		Vector3[] currPoints = new Vector3[points.Count];
		for (int i=0; i<points.Count; i++) {
			currPoints[i] = points[i].localPosition;
		}
		lineRenderer.SetPositions(currPoints);
	}
}
