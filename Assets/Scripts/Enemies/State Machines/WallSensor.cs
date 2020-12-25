using UnityEngine;

public class WallSensor : Sensor {

	public float distance = 1f;
	public bool useTrigger = false;
	public bool autoFlip = false;
	public bool nearWall;

	BoxCollider2D bc;
	int layerMask;

	new void Start() {
		base.Start();
		bc = GetComponent<BoxCollider2D>();
		layerMask = 1 << LayerMask.NameToLayer(Layers.Ground);
	}

	void LateUpdate() {
		Vector2 start = new Vector2(bc.transform.position.x+bc.offset.x, bc.bounds.min.y+0.05f);

		RaycastHit2D hit = Physics2D.Raycast(
			start,
			Vector2.right * e.ForwardScalar(),
			distance,
			layerMask
		);
		Debug.DrawLine(start, start+(Vector2.right * e.ForwardScalar() * distance), Color.red);
		nearWall = (hit.transform != null);
		if (nearWall) {
			Debug.DrawLine(transform.position, hit.point, Color.cyan);
			if (useTrigger) animator.SetTrigger("NearWall");
			if (autoFlip) {
				e.Flip();
			}
		}
		animator.SetBool("NearWall", nearWall);
	}
}
