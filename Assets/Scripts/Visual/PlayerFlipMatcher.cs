using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFlipMatcher : MonoBehaviour {

	GameObject player;
	float originalXScale;

	void Start () {
		player = GlobalController.pc.gameObject;
		originalXScale = transform.localScale.x;
	}
	
	void FixedUpdate() {
		transform.localScale = new Vector2(
			originalXScale * player.transform.localScale.x,
			transform.localScale.y
		);
	}
	
}
