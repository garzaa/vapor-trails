using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallSensor : Sensor {

	public float distance = 1f;
	Vector2 size;
	int layerMask;

	new void Start() {
		base.Start();
		size = GetComponent<BoxCollider2D>().bounds.size * 0.75f;
		layerMask = 1 << LayerMask.NameToLayer(Layers.Ground);
	}

	void Update() {
		RaycastHit2D hit = Physics2D.BoxCast(
			this.transform.position,
			size,
			0,
			Vector2.right * e.ForwardScalar(),
			distance,
			layerMask
		);
		
		animator.SetBool("NearWall", hit.transform != null);
	}
}
