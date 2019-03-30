using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour {

	public GameObject corner1;
	public GameObject corner2;
	public bool generateFromCollider = false;

	protected bool groundedCurrentFrame;
	protected bool ledgeStepCurrentFrame;

	protected void Start() {
		if (generateFromCollider) {
			BoxCollider2D bc = GetComponent<BoxCollider2D>();
			Vector2 center = bc.offset;
			float radiusX = bc.bounds.extents.x;
			float radiusY = bc.bounds.extents.y;
			
			corner1 = new GameObject();
			corner1.name = "corner1";
			corner1.transform.parent = this.transform;
			corner1.transform.localPosition = center - new Vector2(-radiusX, radiusY+0.02f);
			
			corner2 = new GameObject();
			corner2.name = "corner2";
			corner2.transform.parent = this.transform;
			corner2.transform.localPosition = center - new Vector2(radiusX, radiusY+0.02f);
		}
	}

	bool LeftGrounded() {
		Debug.DrawLine(corner1.transform.position + new Vector3(0, 0.1f, 0), corner1.transform.position);
		return Physics2D.Linecast(corner1.transform.position + new Vector3(0, 0.1f, 0), corner1.transform.position, 1 << LayerMask.NameToLayer(Layers.Ground));
	}

	bool RightGrounded() {
		Debug.DrawLine(corner2.transform.position + new Vector3(0, 0.1f, 0), corner2.transform.position);
		return Physics2D.Linecast(corner2.transform.position + new Vector3(0, 0.1f, 0), corner2.transform.position, 1 << LayerMask.NameToLayer(Layers.Ground));
	}

	public bool IsGrounded() {
		return LeftGrounded() || RightGrounded();
	}

	public bool OnLedge() {
		return LeftGrounded() ^ RightGrounded();
	}

	void Update() {
		bool groundedLastFrame = groundedCurrentFrame;
		groundedCurrentFrame = IsGrounded();
		if (!groundedLastFrame && groundedCurrentFrame) {
			GetComponent<Entity>().OnGroundHit();	
		} else if (groundedLastFrame && !groundedCurrentFrame) {
			GetComponent<Entity>().OnGroundLeave();
		}

		if (GetComponent<PlayerController>() != null) {
			bool ledgeStepLastFrame = ledgeStepCurrentFrame;
			ledgeStepCurrentFrame = OnLedge();
			if (!ledgeStepLastFrame && ledgeStepCurrentFrame) {
				GetComponent<PlayerController>().OnLedgeStep();
			}
		}
	}

	public EdgeCollider2D TouchingPlatform() {
		int layerMask = 1 << LayerMask.NameToLayer(Layers.Ground);
		RaycastHit2D g1 = Physics2D.Raycast(corner1.transform.position + new Vector3(0, .2f), Vector3.down, 1f, layerMask);
		RaycastHit2D g2 = Physics2D.Raycast(corner2.transform.position + new Vector3(0, .2f), Vector3.down, 1f, layerMask);
		if (g1.transform == null && g2.transform == null) {
			//return early to avoid redundant checks
			return null;
		}
		bool grounded1 = false;
		bool grounded2 = false;
		
		if (g1.transform != null) {
			grounded1 = g1.transform.gameObject.GetComponent<PlatformEffector2D>() != null;
		}
		if (g2.transform != null) {
			grounded2 = g2.transform.gameObject.GetComponent<PlatformEffector2D>() != null;
		}
		
		if (grounded1 && grounded2) {
			return g2.transform.gameObject.GetComponent<EdgeCollider2D>();
		}
		return null;
	}
}
