using UnityEngine;

public class WallSensor : Sensor {

	public float distance = 1f;
	public bool useTrigger = false;
	public bool autoFlip = false;
	Vector2 size;
	int layerMask;
	Entity entity;

	new void Start() {
		base.Start();
		entity = animator.GetComponent<Entity>();
		size = GetComponent<BoxCollider2D>().bounds.size * 0.35f;
		layerMask = 1 << LayerMask.NameToLayer(Layers.Ground);
	}

	void Update() {
		Debug.DrawLine(
			this.transform.position,
			this.transform.position + new Vector3((Vector2.right * e.ForwardScalar()).x, (Vector2.right * e.ForwardScalar()).y),
			Color.red
		);
		Debug.DrawLine(transform.position, transform.position - (Vector3) size);
		RaycastHit2D hit = Physics2D.BoxCast(
			this.transform.position,
			size,
			0,
			Vector2.right * e.ForwardScalar(),
			distance,
			layerMask
		);
		
		if (hit.transform != null && useTrigger) {
			animator.SetTrigger("NearWall");
		}
		animator.SetBool("NearWall", hit.transform != null);
		if (autoFlip) {
			entity.Flip();
		}
	}
}
