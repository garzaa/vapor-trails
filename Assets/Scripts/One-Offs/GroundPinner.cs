using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundPinner : MonoBehaviour {

	public GameObject target;
	public Vector2 direction;
	public float maxDistance = 64;

	void Update() {
		RaycastHit2D hit = Physics2D.Raycast(
			transform.position, 
			direction.normalized, 
			maxDistance,
			1 << LayerMask.NameToLayer(Layers.Ground)
		);
		if (hit.transform != null) {
			target.SetActive(true);
			target.transform.position = hit.point;
		} else {
			target.SetActive(false);
		}
	}
}
