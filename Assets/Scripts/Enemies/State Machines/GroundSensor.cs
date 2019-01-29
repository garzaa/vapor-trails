using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundSensor : Sensor {

	public string floatName = "GroundDistance";
	public Vector2 direction;

	void Update () {
		Debug.DrawLine(this.transform.position, new Vector2(this.transform.position.x, this.transform.position.y) + (direction * 64));
		RaycastHit2D hit = Physics2D.Raycast(this.transform.position, direction, 64, 1 << LayerMask.NameToLayer(Layers.Ground));
		if (hit.transform != null) {
			animator.SetFloat(floatName, hit.distance);
		}
	}
}
