using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCheck : MonoBehaviour {

	#pragma warning disable 0649
	[SerializeField] BoxCollider2D targetCollider;
	[SerializeField] float groundGap = 0.06f;
	#pragma warning restore 0649
	const bool drawDebug = true;

	public bool touchingWall;

	int layerMask;

	const float extendDistance = 0.02f;

	void Start() {		
		layerMask = 1 << LayerMask.NameToLayer(Layers.Ground);
	}

	public WallCheckData GetWall() {
		Vector2 startPoint = (Vector2) targetCollider.transform.position + targetCollider.offset;
		Vector2 actualSize = new Vector2(targetCollider.size.x, targetCollider.bounds.size.y-(2*groundGap));

		float distance = targetCollider.size.x/2f + extendDistance;

		Vector2 topStart = startPoint+(Vector2.up*actualSize.y*0.5f);
		Vector2 bottomStart = startPoint+(Vector2.down*actualSize.y*0.5f);

		//cast left
		RaycastHit2D topHit = Physics2D.Raycast(
			origin: topStart,
			direction: Vector2.left,
			distance: distance,
			layerMask: layerMask
		);
		RaycastHit2D bottomHit = Physics2D.Raycast(
			origin: bottomStart,
			direction: Vector2.left,
			distance: distance,
			layerMask: layerMask
		);
		Debug.DrawLine(topStart, topStart+(Vector2.left*distance), Color.cyan);
		Debug.DrawLine(bottomStart, bottomStart+(Vector2.left*distance), Color.cyan);
		if (topHit.collider!=null || bottomHit.collider!=null) {
			touchingWall = true;
			return new WallCheckData(
				Vector2.Distance(startPoint, topHit.point),
				-1
			);
		}

		// cast right
		topHit = Physics2D.Raycast(
			origin: startPoint+(Vector2.up*actualSize.y*0.5f),
			direction: Vector2.right,
			distance: distance,
			layerMask: layerMask
		);
		bottomHit = Physics2D.Raycast(
			origin: startPoint+(Vector2.down*actualSize.y*0.5f),
			direction: Vector2.right,
			distance: distance,
			layerMask: layerMask
		);
		if (topHit.collider!=null || bottomHit.collider!=null) {
			touchingWall = true;
			return new WallCheckData(
				Vector2.Distance(startPoint, topHit.point),
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
