using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateOnInteract : Interactable {

	public Activatable activatable;

	public override void Interact(GameObject player) {
		activatable.ActivateSwitch(true);
	}
}
