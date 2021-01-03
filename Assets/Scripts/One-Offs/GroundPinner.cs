using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GroundPinner : MonoBehaviour {

	public GameObject target;
	public Vector2 direction;
	public float maxDistance = 10;
	public bool disableIfMiss = true;

	void Update() {
		RaycastHit2D hit = Physics2D.Raycast(
			transform.position, 
			direction.normalized.Rotate(this.transform.eulerAngles.z), 		
			maxDistance,
			1 << LayerMask.NameToLayer(Layers.Ground)
		);
		if (hit.transform != null) {
			if (disableIfMiss) target.SetActive(true);
			target.transform.position = hit.point;
		} else {
			if (disableIfMiss) target.SetActive(false);
			target.transform.position = this.transform.position + (((Vector3) direction.normalized.Rotate(transform.eulerAngles.z)) * maxDistance);
		}
	}
}
