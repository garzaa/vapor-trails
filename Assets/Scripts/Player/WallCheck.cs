using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCheck : MonoBehaviour {

	public Transform topCorner;
	public Transform bottomCorner;

	public GameObject CheckPoint(Transform corner) {
		Vector2 start = new Vector2(transform.position.x, corner.position.y);
		Debug.DrawLine(start, corner.position, Color.red);
		RaycastHit2D hit = Physics2D.Linecast(start, corner.position, 1 << LayerMask.NameToLayer(Layers.Ground));
		if (hit.transform != null) {
			return hit.transform.gameObject;
		}
		return null;
	}

	public GameObject TouchingWall() {
		GameObject topGrounded = CheckPoint(topCorner);
		GameObject bottomGrounded = CheckPoint(bottomCorner);
		if (topGrounded != null && bottomGrounded != null) {
			return topGrounded;
		}
		return null;
	}

	public bool TouchingLedge() {
		return !CheckPoint(topCorner) && CheckPoint(bottomCorner);
	}
}
