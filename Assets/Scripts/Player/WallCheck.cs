using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCheck : MonoBehaviour {

	public Transform topCorner;
	public Transform bottomCorner;

	public bool CheckPoint(Transform corner) {
		return Physics2D.Linecast(transform.position, corner.position, 1 << LayerMask.NameToLayer(Layers.Ground));
	}

	public bool TouchingWall() {
		bool topGrounded = CheckPoint(topCorner);
		bool bottomGrounded = CheckPoint(bottomCorner);

		return topGrounded && bottomGrounded;
	}

	public bool TouchingLedge() {
		return !CheckPoint(topCorner) && CheckPoint(bottomCorner);
	}
}
