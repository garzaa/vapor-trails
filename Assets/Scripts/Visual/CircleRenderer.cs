using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(LineRenderer))]
public class CircleRenderer : MonoBehaviour {

	public int segments;
	public float radius = 10f;

	int segmentsLastFrame;
	float radiusLastFrame;
	LineRenderer line;

	void Start() {
		line = GetComponent<LineRenderer>();
		DrawCircle(segments);
	}
	
	void Update () {
		if (Changed() && segments > 0) {
			DrawCircle(segments);
		}
		segmentsLastFrame = segments;
		radiusLastFrame = radius;
	}

	bool Changed() {
		return segments != segmentsLastFrame || radius != radiusLastFrame;
	}

	void DrawCircle(int segments) {
		Vector3[] points = new Vector3[segments];
		line.positionCount = segments;
		for (int i=0; i<segments; i++) {
			float angle =  ((float) i/segments) * Mathf.PI*2.0f;
			points[i] = new Vector3(
				Mathf.Sin(angle)*radius, 
				Mathf.Cos(angle)*radius, 
				0
			);
		}
		line.SetPositions(points);
	}
}
