using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGrabber : MonoBehaviour {

	void OnCollisionStay2D(Collision2D c) {
		if (c.gameObject.tag == "Player") {
			c.transform.parent = this.transform;
		}
	}

	void OnCollisionLeave2D(Collision2D c) {
		if (c.gameObject.tag == "Player") {
			c.transform.parent = null;
		}
	}
}