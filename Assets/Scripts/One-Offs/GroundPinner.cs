using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundPinner : MonoBehaviour {

	public GameObject target;

	void Update() {
		Debug.DrawLine(transform.position, Vector2.down * 128f);
		RaycastHit2D hit = Physics2D.Raycast(
			transform.position, 
			Vector2.down, 
			128f,
			1 << LayerMask.NameToLayer(Layers.Ground)
		);
		if (hit.transform != null) {
			target.SetActive(true);
			target.transform.localPosition = hit.transform.localPosition;
		} else {
			target.SetActive(false);
		}
	}
}
