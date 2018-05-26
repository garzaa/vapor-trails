using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour {

	public Transform corner1;
	public Transform corner2;
	public bool generateFromCollider = false;

	bool isGrounded;

	void Start() {
		if (generateFromCollider) {
			BoxCollider2D bc = GetComponent<BoxCollider2D>();
			Vector2 center = bc.offset;
			float radiusX = bc.bounds.extents.x;
			float radiusY = bc.bounds.extents.y;
			
			corner1 = new GameObject().transform;
			corner1.position = center - new Vector2(-radiusX, radiusY);
			
			corner2 = new GameObject().transform;
			corner2.position = center - new Vector2(radiusX, radiusY);
		}
	}

	public bool IsGrounded() {
		bool leftGrounded = Physics2D.Linecast(transform.position, corner1.position, 1 << LayerMask.NameToLayer(Layers.Ground));
		bool rightGrounded = Physics2D.Linecast(transform.position, corner2.position, 1 << LayerMask.NameToLayer(Layers.Ground));
		return leftGrounded || rightGrounded;
	}
}
