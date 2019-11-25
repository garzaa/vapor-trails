using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForcedInteractTrigger : PlayerTriggeredObject {

	public Interactable i;
	bool interactedOnce;
	public bool unpairAfterInteract = true;

	override protected void Start() {
		base.Start();
		if (i == null) {
			i = GetComponentInChildren<Interactable>();
		}
		//keep the player from interacting with it somehow, unless it's the actual interaction trigger
		if (i.GetComponent<BoxCollider2D>() != null && i.transform != this.transform) {
			i.gameObject.GetComponent<BoxCollider2D>().enabled = false;
		}
	}

	public override void OnPlayerEnter() {
		if (!interactedOnce) {
			i.Interact(this.player.gameObject);
			interactedOnce = true;

			if (unpairAfterInteract) {
				if (i.GetComponent<BoxCollider2D>() != null) {
					i.gameObject.GetComponent<BoxCollider2D>().enabled = true;
				}
				i.transform.parent = null;
				Destroy(this.gameObject);
			}
		}
	}

	public override void OnPlayerExit() {
		
	}
}
