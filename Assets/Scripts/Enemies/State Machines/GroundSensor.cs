using UnityEngine;

public class GroundSensor : Sensor {

	public string floatName = "GroundDistance";
	public Vector2 direction;

	void Update () {
		RaycastHit2D hit = Physics2D.Raycast(this.transform.position, direction, 64, 1 << LayerMask.NameToLayer(Layers.Ground));
		if (hit.transform != null) {
			animator.SetFloat(floatName, hit.distance);
		}
	}
}
