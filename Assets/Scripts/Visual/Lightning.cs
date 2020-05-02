using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(LineRenderer))]
public class Lightning : MonoBehaviour {

	public Transform pointA;
	public Transform pointB;
	public float segmentsPerUnit = 1;
	[Range(0, 1)]
	public float perturbation = 0.5f;

	[Range(1, 60)]
	public float fps = 30;

	LineRenderer lineRenderer;

	EdgeCollider2D edge;

	void OnEnable() {
		lineRenderer = GetComponent<LineRenderer>();
		if (pointA == null || pointB == null) {
			Debug.Log("brainlet alert");
		}
		StartCoroutine(WaitAndDraw());
		edge = GetComponent<EdgeCollider2D>();
	}

	IEnumerator WaitAndDraw() {
		yield return new WaitForSeconds(1f / (float) fps);
		Draw();
	}


	void Draw() {
		float distance = Vector2.Distance(pointA.position, pointB.position);
		if (distance < 0.01f) return;
		// don't ask. don't change
		distance += 1.5f;
		//want your linerenderer to use world space? too bad idiot, you have to work for it
		int numPoints = (int) (distance * segmentsPerUnit);
		if (numPoints < 1) {
			return;
		}
		//the fence post error
		Vector3[] points = new Vector3[numPoints+1];
		//then move from A to B, with some noise
		float angle = Vector2.Angle(pointA.position, pointB.position);
		for (int i=0; i<numPoints+1; i++) {
			points[i] = Vector2.MoveTowards(
				pointA.position, 
				pointB.position,
				i * (distance / numPoints)
			) + Perturbator(i, numPoints, angle);
		}
		lineRenderer.positionCount = numPoints;
		lineRenderer.SetPositions(points);
		StartCoroutine(WaitAndDraw());
		if (edge != null) SetEdgePositions();
	}

	Vector2 Perturbator(int i, int numPoints, float originalAngle) {
		//first and last points should always be consistent
		if (i==0 || i==numPoints-1 || perturbation == 0) {
			return Vector2.zero;
		} else {
			//we only want noise perpendicular to A->B
			Vector2 originalVector = new Vector2(
				Random.Range(-perturbation, perturbation),
				0
			).Rotate(90);
			return originalVector;
		}
	}

	void SetEdgePositions() {
		edge.points = new Vector2[] {pointA.transform.position, pointB.transform.position};
	}
}
