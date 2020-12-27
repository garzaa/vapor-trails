using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateOnInteract : Interactable {

	public Activatable activatable;
	public bool wallButton;

	public override void Interact(GameObject player) {
		base.Interact(player);
		if (wallButton) {
			player.GetComponent<Animator>().SetTrigger("PressWallButton");
			StartCoroutine(ButtonTimeInteract());
		} else {
			activatable.ActivateSwitch(true);
		}
	}

	IEnumerator ButtonTimeInteract() {
		yield return new WaitForSecondsRealtime(0.25f);
		activatable.ActivateSwitch(true);
	}
}
