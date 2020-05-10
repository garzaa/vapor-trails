using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCheck : MonoBehaviour {

	[SerializeField] BoxCollider2D targetCollider;
	[SerializeField] float castDistance = 0.01f;
	[SerializeField] float toeGap = 0.06f;
	[SerializeField] bool drawDebug = false;

	public bool touchingWall;

	public WallCheckData GetWall() {
		Vector2 startPoint = new Vector2(targetCollider.bounds.center.x, targetCollider.bounds.center.y+toeGap);
		Vector2 actualSize = new Vector2(targetCollider.bounds.size.x, targetCollider.bounds.size.y-toeGap);

		Debug.DrawLine(startPoint+actualSize/2, startPoint-actualSize/2, Color.blue);
		int layerMask = 1 << LayerMask.NameToLayer(Layers.Ground);

		// cast left and right
		RaycastHit2D hit = Physics2D.BoxCast(startPoint, actualSize, 0, Vector2.left, castDistance, layerMask);
		if (drawDebug) Debug.DrawLine(startPoint, startPoint + Vector2.left*(actualSize.x/2f + castDistance), Color.red);
		if (hit.transform != null) {
			if (drawDebug) {
				Debug.DrawLine(startPoint, hit.transform.position, Color.magenta);
			}
			touchingWall = true;
			return new WallCheckData(
				Vector2.Distance(startPoint, hit.transform.position),
				-1
			);
		}

		hit = Physics2D.BoxCast(startPoint, actualSize, 0, Vector2.right, castDistance, layerMask);
		if (drawDebug) Debug.DrawLine(startPoint, startPoint + Vector2.right*(actualSize.x/2f+castDistance), Color.green);
		if (hit.transform != null) {
			touchingWall = true;
			return new WallCheckData(
				Vector2.Distance(startPoint, hit.transform.position),
				-1
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
