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
	}

	void Update() {
		Vector3[] currPoints = new Vector3[points.Count];
		for (int i=0; i<points.Count; i++) {
			//don't immediately start throwing errors when it's first created in the editor
			if (points[i] == null) {
				return;
			}
			currPoints[i] = points[i].localPosition;
		}
		lineRenderer.SetPositions(currPoints);	
	}
}
