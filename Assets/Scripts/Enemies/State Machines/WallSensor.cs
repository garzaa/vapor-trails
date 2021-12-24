using UnityEngine;

public class WallSensor : Sensor {

	public float distance = 1f;
	public bool useTrigger = false;
	public bool autoFlip = false;
	public bool nearWall;

	Collider2D col;
	int layerMask;

	new void Start() {
		base.Start();
		col = GetComponent<Collider2D>();
		layerMask = 1 << LayerMask.NameToLayer(Layers.Ground);
	}

	void LateUpdate() {
		Vector2 start = new Vector2(col.transform.position.x+col.offset.x, col.bounds.min.y+0.05f);

		RaycastHit2D hit = Physics2D.Raycast(
			start,
			transform.TransformDirection(Vector2.right * e.ForwardScalar()),
			distance,
			layerMask
		);
		Vector2 raycast = transform.TransformDirection(Vector2.right * e.ForwardScalar() * distance);
		Debug.DrawLine(start, start+raycast, Color.red);
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
