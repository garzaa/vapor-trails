using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(LineRenderer))]
public class CircleRenderer : LineRendererEditor {

	public int segments;
	[Range(0, 1)]
	public float arcFraction = 1f;
	public float radius = .10f;

	int segmentsLastFrame;
	float radiusLastFrame;
	float arcFractionLastFrame;

	override protected void Start() {
		base.Start();
		DrawCircle(segments);
	}
	
	void LateUpdate () {
		if (Changed() && segments > 0) {
			DrawCircle(segments);
		}
		segmentsLastFrame = segments;
		radiusLastFrame = radius;
		arcFractionLastFrame = arcFraction;
	}

	bool Changed() {
		return segments != segmentsLastFrame || radius != radiusLastFrame || arcFraction != arcFractionLastFrame;
	}

	void DrawCircle(int segments) {
		Vector3[] points = new Vector3[segments];
		int actualSegments = (int) ((float) segments * arcFraction); 
		line.positionCount = actualSegments;
		for (int i=0; i<actualSegments; i++) {
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
