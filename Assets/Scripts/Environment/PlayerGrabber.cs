using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGrabber : MonoBehaviour {

	Transform lastParent;

	void OnCollisionEnter2D(Collision2D c) {
		if (c.gameObject.CompareTag(Tags.Player)) {
			lastParent = c.transform.parent;
			c.transform.parent = this.transform;
		}
	}

	void OnCollisionExit2D(Collision2D c) {
		if (c.gameObject.CompareTag(Tags.Player)) {
			AlerterText.Alert("benis");
			c.transform.parent = lastParent;
		}
	}
}