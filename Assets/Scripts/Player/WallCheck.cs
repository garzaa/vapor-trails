using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCheck : MonoBehaviour {

	[SerializeField] BoxCollider2D targetCollider;
	[SerializeField] float groundGap = 0.06f;
	const bool drawDebug = true;

	public bool touchingWall;
	ContactFilter2D filter;

	int numHits;
	RaycastHit2D[] hits = new RaycastHit2D[1];
	RaycastHit2D hit;

	const float extendDistance = 0.01f;
	// TODO: what the fuck? redo this the right way with normals logging
	// ok so normals are either (-1, 0) for a left wall or (1, 0) for a right wall
	// what the fuck angle does that translate to
	// ok it's the angle against the cast distance
	const float normalTolerance = 5f;

	void Start() {
		filter = new ContactFilter2D();
		filter.layerMask = 1 << LayerMask.NameToLayer(Layers.Ground);
		filter.useLayerMask = true;
	}

	public WallCheckData GetWall() {
		Vector2 startPoint = (Vector2) targetCollider.transform.position + targetCollider.offset;
		Vector2 actualSize = new Vector2(targetCollider.size.x, targetCollider.bounds.size.y-(2*groundGap));

		Debug.DrawLine(startPoint+actualSize/2, startPoint-actualSize/2, Color.blue);

		float distance = targetCollider.size.x/2f + extendDistance;

		// cast left and right
		numHits = Physics2D.BoxCast(startPoint, actualSize, 0, Vector2.left, filter, hits, distance);
		if (drawDebug) {
			// top edge
			Debug.DrawLine(
				new Vector2(startPoint.x, startPoint.y + (targetCollider.size.y/2f)),
				new Vector2(startPoint.x-distance, startPoint.y + (targetCollider.size.y/2f)),
				Color.red
			);
			// bottom edge
			Debug.DrawLine(
				new Vector2(startPoint.x, startPoint.y - (targetCollider.size.y/2f)),
				new Vector2(startPoint.x-distance, startPoint.y - (targetCollider.size.y/2f)),
				Color.red
			);
		}
		if (numHits != 0) {
			hit = hits[0];
			touchingWall = true;
			return new WallCheckData(
				Vector2.Distance(startPoint, hit.transform.position),
				-1
			);
		}

		// right
		numHits = Physics2D.BoxCast(startPoint, actualSize, 0, Vector2.right, filter, hits, distance);
		if (drawDebug) Debug.DrawLine(startPoint, startPoint + Vector2.right*(actualSize.x/2f+distance), Color.green);
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
