using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForcedInteractTrigger : PlayerTriggeredObject {

	Interactable i;

	void Start() {
		i = GetComponentInChildren<Interactable>();
		//keep the plater from interacting with it somehow
		i.gameObject.GetComponent<BoxCollider2D>().enabled = false;
	}

	public override void OnPlayerEnter() {
		i.Interact(player.gameObject);
	}

	public override void OnPlayerExit() {

	}
}
