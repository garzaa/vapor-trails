using UnityEngine;
using System.Collections;

public class GroundCheck : MonoBehaviour {

	public GameObject corner1;
	public GameObject corner2;
	public bool generateFromCollider = false;
	public GameObject currentGround;

	bool groundedCurrentFrame;
	bool ledgeStepCurrentFrame;

	float raycastLength = 0.4f;
	float impactSpeed = 0f;

	Rigidbody2D rb2d;
	protected Entity entity;

	int defaultLayerMask;

	public bool constantlyUseCollider = false;
	public BoxCollider2D collidertoUse;

	PlayerController player;

	protected void Start() {
		defaultLayerMask = 1 << LayerMask.NameToLayer(Layers.Ground);
		if (generateFromCollider && !constantlyUseCollider) {
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
		entity = GetComponent<Entity>();
		if (entity != null) {
			rb2d = entity.GetComponent<Rigidbody2D>();
		}
		if (GetComponent<PlayerController>() != null) {
			player = GetComponent<PlayerController>();
		}
	}

	bool LeftGrounded() {
		Vector2 pos = constantlyUseCollider ? collidertoUse.BottomLeftCorner() : (Vector2) corner1.transform.position;
		RaycastHit2D hit = DefaultLinecast(pos);
		if (hit) {
			currentGround = hit.collider.gameObject;
		}
		return hit;
	}

	bool RightGrounded() {
		Vector2 pos = constantlyUseCollider ? collidertoUse.BottomRightCorner() : (Vector2) corner2.transform.position;
		return DefaultLinecast(pos);
	}

	public bool IsGrounded() {
		if (!constantlyUseCollider) return LeftGrounded() || RightGrounded();
		else {
			Vector2 origin = (Vector2) collidertoUse.transform.position + collidertoUse.offset;
			origin.y -= collidertoUse.size.y/2f;
			RaycastHit2D hit = Physics2D.BoxCast(
				origin,
				collidertoUse.size * new Vector2(0.9f, .1f),
				0f,
				Vector2.down,
				0.05f,
				defaultLayerMask
			);
			return hit.transform != null;
		}
	}

	public bool OnLedge() {
		return LeftGrounded() ^ RightGrounded();
	}

	void Update() {
		bool groundedLastFrame = groundedCurrentFrame;
		groundedCurrentFrame = IsGrounded();
		if (!groundedLastFrame && groundedCurrentFrame) {
			entity.OnGroundHit(impactSpeed);	
		} else if (groundedLastFrame && !groundedCurrentFrame) {
			entity.OnGroundLeave();
		}

		if (player != null) {
			bool ledgeStepLastFrame = ledgeStepCurrentFrame;
			ledgeStepCurrentFrame = OnLedge();

			if (!ledgeStepLastFrame && ledgeStepCurrentFrame) {
				GetComponent<PlayerController>().OnLedgeStep();
			}
		}
		if (rb2d != null) {
			impactSpeed = rb2d.velocity.y;
		}
	}

	public EdgeCollider2D[] TouchingPlatforms() {
		Vector2 pos1 = constantlyUseCollider ? collidertoUse.BottomLeftCorner() : (Vector2) corner1.transform.position;
		Vector2 pos2 = constantlyUseCollider ? collidertoUse.BottomRightCorner() : (Vector2) corner2.transform.position;
		RaycastHit2D g1 = DefaultLinecast(pos1);
		RaycastHit2D g2 = DefaultLinecast(pos2);
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
			return new EdgeCollider2D[] {
				g1.transform.gameObject.GetComponent<EdgeCollider2D>(),
				g2.transform.gameObject.GetComponent<EdgeCollider2D>()
			};
		}
		return null;
	}

	Vector3 GetOffset() {
		return new Vector3(0, raycastLength, 0);
	}

	RaycastHit2D DefaultLinecast(Vector2 cornerPos) {
		if (!constantlyUseCollider) {
			Debug.DrawLine(cornerPos, cornerPos + (Vector2) GetOffset());
			return Physics2D.Linecast(
				cornerPos + (Vector2) GetOffset(),
				cornerPos,
				1 << LayerMask.NameToLayer(Layers.Ground)
			);
		}
		else {
			// start 15 pixels above the collider edge
			// rotate to keep in line with rigidbody
			Vector2 margin = Vector2.up*0.05f;
			// cornerPos is passed in in world space
			cornerPos = transform.InverseTransformPoint(cornerPos);
			Vector2 start = transform.TransformPoint(cornerPos + margin);
			Vector2 end = transform.TransformPoint(cornerPos - margin);

			Debug.DrawLine(start, end);
			return Physics2D.Linecast(
				start,
				end,
				1 << LayerMask.NameToLayer(Layers.Ground)
			);
		}
	}
}
