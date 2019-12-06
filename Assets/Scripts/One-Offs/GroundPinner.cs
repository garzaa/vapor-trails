using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GroundPinner : MonoBehaviour {

	public GameObject target;
	public Vector2 direction;
	public float maxDistance = 64;

	void Update() {
		RaycastHit2D hit = Physics2D.Raycast(
			transform.position, 
			direction.normalized.Rotate(this.transform.rotation.z), 
			maxDistance,
			1 << LayerMask.NameToLayer(Layers.Ground)
		);
		if (hit.transform != null) {
			Debug.DrawLine(this.transform.position, hit.point, Color.green);
			target.SetActive(true);
			target.transform.position = hit.point;
		} else {
			target.SetActive(false);
			Debug.DrawRay(this.transform.position, direction.Rotate(this.transform.rotation.z), Color.red);
		}
	}
}
