using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForcedInteractTrigger : PlayerTriggeredObject {

	Interactable i;
	bool interactedOnce;
	public bool unpairAfterInteract = true;

	void Start() {
		i = GetComponentInChildren<Interactable>();
		//keep the player from interacting with it somehow
		i.gameObject.GetComponent<BoxCollider2D>().enabled = false;
	}

	public override void OnPlayerEnter() {
		if (!interactedOnce) {
			i.Interact(
				this.player.gameObject);
			interactedOnce = true;

			if (unpairAfterInteract) {
				i.gameObject.GetComponent<BoxCollider2D>().enabled = true;
				i.transform.parent = null;
				Destroy(this.gameObject);
			}
		}
	}

	public override void OnPlayerExit() {

	}
}
