using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGrabber : MonoBehaviour {

	Transform lastParent;

	void OnCollisionEnter2D(Collision2D c) {
		if (c.gameObject.CompareTag(Tags.Player)) {
			lastParent = c.transform.parent;
			c.transform.parent = this.transform;
			GlobalController.pc.OnGrab(this);
		}
	}

	void OnCollisionExit2D(Collision2D c) {
		if (c.gameObject.CompareTag(Tags.Player)) {
			c.transform.parent = lastParent;
			ReleasePlayer();
		}
	}

	public void ReleasePlayer() {
		GlobalController.pc.transform.parent = lastParent;
		GlobalController.pc.OnGrabRelease();
	}
}
