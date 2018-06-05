using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerTriggeredObject : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D other) {
		if (other.CompareTag(Tags.Player)) {
			OnPlayerEnter();
		}
	}

	void OnTriggerExit2D(Collider2D other) {
		if (other.CompareTag(Tags.Player)) {
			OnPlayerExit();
		}
	}

	public abstract void OnPlayerEnter();

	public abstract void OnPlayerExit();
}
