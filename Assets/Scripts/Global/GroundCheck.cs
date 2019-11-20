using UnityEngine;
using System.Collections;

public class GroundCheck : MonoBehaviour {

	public GameObject corner1;
	public GameObject corner2;
	public bool generateFromCollider = false;
	public GameObject currentGround;

	bool groundedCurrentFrame;
	bool ledgeStepCurrentFrame;

	float coyoteTime = 000f;
	float raycastLength = 0.4f;

	int layerMask;

	protected void Start() {
		layerMask = 1 << LayerMask.NameToLayer(Layers.Ground);
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
		RaycastHit2D hit = DefaultLinecast(corner1);
		if (hit) {
			currentGround = hit.collider.gameObject;
		}
		return hit;
	}

	bool RightGrounded() {
		return DefaultLinecast(corner2);
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
			StartCoroutine(GroundLeaveTimeout(coyoteTime));
		}

		if (GetComponent<PlayerController>() != null) {
			bool ledgeStepLastFrame = ledgeStepCurrentFrame;
			ledgeStepCurrentFrame = OnLedge();
			if (!ledgeStepLastFrame && ledgeStepCurrentFrame) {
				GetComponent<PlayerController>().OnLedgeStep();
			}
		}
	}

	IEnumerator GroundLeaveTimeout(float interval) {
		yield return new WaitForSecondsRealtime(interval);
		GetComponent<Entity>().OnGroundLeave();
	}

	public EdgeCollider2D[] TouchingPlatforms() {
		RaycastHit2D g1 = DefaultLinecast(corner1);
		RaycastHit2D g2 = DefaultLinecast(corner2);
		Debug.Log("checking for platforms");
		if (g1.transform == null && g2.transform == null) {
			//return early to avoid redundant checks
			print("not touching anything");
			return null;
		}
		bool grounded1 = false;
		bool grounded2 = false;
		
		if (g1.transform != null) {
			Debug.Log(g1.transform.gameObject.name);
			Debug.DrawRay(g1.transform.position, Vector3.up, Color.red, 1);
			grounded1 = g1.transform.gameObject.GetComponent<PlatformEffector2D>() != null;
		}
		if (g2.transform != null) {
			grounded2 = g2.transform.gameObject.GetComponent<PlatformEffector2D>() != null;
		}
		
		if (grounded1 || grounded2) {
			return new EdgeCollider2D[] {
				g1.transform.gameObject.GetComponent<EdgeCollider2D>(),
				g2.transform.gameObject.GetComponent<EdgeCollider2D>()
			};
		}
		return null;
	}

	public float GetGroundDifference() {
		RaycastHit2D hit = DefaultLinecast(corner1);
		if (hit.transform == null) return 0;
		Debug.Log(hit.distance);
		// the raycast extends a bit below the collider's min bounds
		return hit.distance;
	}

	Vector3 GetOffset() {
		return new Vector3(0, raycastLength, 0);
	}

	RaycastHit2D DefaultLinecast(GameObject corner) {
		return Physics2D.Linecast(
			corner.transform.position + GetOffset(),
			corner.transform.position,
			1 << LayerMask.NameToLayer(Layers.Ground)
		);
	}
}
