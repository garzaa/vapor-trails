using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFlipMatcher : MonoBehaviour {

	GameObject player;

	void Start () {
		if (player == null) {
			player = GameObject.Find("Player");
		}
	}
	
	void FixedUpdate() {
		this.transform.localScale = player.transform.localScale;
	}
	
}
