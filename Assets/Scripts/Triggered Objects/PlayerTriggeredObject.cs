using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerTriggeredObject : MonoBehaviour {

	[HideInInspector]
	public PlayerController player;

	void Start() {
		gameObject.layer = LayerMask.NameToLayer(Layers.Triggers);
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.CompareTag(Tags.Player)) {
			this.player = other.GetComponent<PlayerController>();
			OnPlayerEnter();
		}
	}

	void OnTriggerExit2D(Collider2D other) {
		if (other.CompareTag(Tags.Player)) {
			OnPlayerExit();
			this.player = null;
		}
	}

	public abstract void OnPlayerEnter();

	public abstract void OnPlayerExit();
}
