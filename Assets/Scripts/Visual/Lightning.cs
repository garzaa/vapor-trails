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

	void Start() {
		lineRenderer = GetComponent<LineRenderer>();
		if (pointA == null || pointB == null) {
			Debug.Log("brainlet alert");
		}
		StartCoroutine(WaitAndDraw());
	}

	IEnumerator WaitAndDraw() {
		yield return new WaitForSeconds(1f / (float) fps);
		Draw();
	}


	void Draw() {
		float distance = Vector2.Distance(pointA.localPosition, pointB.localPosition);
		//want your linerenderer to use world space? too bad idiot, you have to work for it
		Vector2 normB = pointB.localPosition - pointA.localPosition;
		int numPoints = (int) (distance * segmentsPerUnit);
		print(numPoints);
		print(distance * segmentsPerUnit);
		if (numPoints < 1) {
			print("bengis");
			return;
		}
		//the fence post error
		Vector3[] points = new Vector3[numPoints+1];
		//then move from A to B, with some noise
		float angle = Vector2.Angle(pointA.localPosition, normB);
		for (int i=0; i<numPoints+1; i++) {
			points[i] = Vector2.MoveTowards(
				pointA.localPosition, 
				normB,
				i * (distance / numPoints)
			) + Perturbator(i, numPoints, angle);
		}
		lineRenderer.positionCount = numPoints;
		lineRenderer.SetPositions(points);
		StartCoroutine(WaitAndDraw());
	}

	Vector2 Perturbator(int i, int numPoints, float originalAngle) {
		//first and last points should always be consistent
		if (i==0 || i==numPoints) {
			return Vector2.zero;
		} else {
			//we only want noise perpendicular to A->B
			Vector2 originalVector = new Vector2(
				Random.Range(-perturbation, perturbation),
				0
			);
			float newAngle = Mathf.PI - originalAngle;
			return Quaternion.AngleAxis(newAngle, Vector3.back) * originalVector;
		}
	}
}
