using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCheck : MonoBehaviour {

	[SerializeField] BoxCollider2D targetCollider;
	[SerializeField] float castDistance = 0.01f;
	[SerializeField] float groundGap = 0.06f;
	[SerializeField] bool drawDebug = false;

	public bool touchingWall;
	ContactFilter2D filter;

	int numHits;
	RaycastHit2D[] hits = new RaycastHit2D[1];
	RaycastHit2D hit;

	void Start() {
		filter = new ContactFilter2D();
		// no upward-facing normals, so no platforms
		filter.SetNormalAngle(0, 180);
		filter.layerMask = 1 << LayerMask.NameToLayer(Layers.Ground);
		filter.useLayerMask = true;
	}

	public WallCheckData GetWall() {
		Vector2 startPoint = new Vector2(targetCollider.bounds.center.x, targetCollider.bounds.center.y);
		Vector2 actualSize = new Vector2(targetCollider.bounds.size.x, targetCollider.bounds.size.y-(2*groundGap));

		Debug.DrawLine(startPoint+actualSize/2, startPoint-actualSize/2, Color.blue);
		int layerMask = 1 << LayerMask.NameToLayer(Layers.Ground);

		// cast left and right
		// left
		numHits = Physics2D.BoxCast(startPoint, actualSize, 0, Vector2.left, filter, hits, castDistance);
		if (drawDebug) Debug.DrawLine(startPoint, startPoint + Vector2.left*(actualSize.x/2f + castDistance), Color.red);
		if (numHits != 0) {
			hit = hits[0];
			if (drawDebug) {
				Debug.DrawLine(startPoint, hit.transform.position, Color.magenta);
			}
			touchingWall = true;
			return new WallCheckData(
				Vector2.Distance(startPoint, hit.transform.position),
				-1
			);
		}

		// right
		numHits = Physics2D.BoxCast(startPoint, actualSize, 0, Vector2.right, filter, hits, castDistance);
		if (drawDebug) Debug.DrawLine(startPoint, startPoint + Vector2.right*(actualSize.x/2f+castDistance), Color.green);
		if (numHits != 0) {
			hit = hits[0];
			touchingWall = true;
			if (drawDebug) {
				Debug.DrawLine(startPoint, hit.transform.position, Color.magenta);
			}
			return new WallCheckData(
				Vector2.Distance(startPoint, hit.transform.position),
				1
			);
		}

		touchingWall = false;
		return null;
	}
}

public class WallCheckData {
	public float distance;
	public int direction;

	public WallCheckData(float dist, int dir) {
		this.distance = dist;
		this.direction = dir;
	}
}
