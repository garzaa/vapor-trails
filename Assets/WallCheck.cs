using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCheck : MonoBehaviour {

	public Transform topCorner;
	public Transform bottomCorner;

	public bool TouchingWall() {
		bool topGrounded = Physics2D.Linecast(transform.position, topCorner.position, 1 << LayerMask.NameToLayer(Layers.Ground));
		bool bottomGrounded = Physics2D.Linecast(transform.position, bottomCorner.position, 1 << LayerMask.NameToLayer(Layers.Ground));

		return topGrounded && bottomGrounded;
	}
}
