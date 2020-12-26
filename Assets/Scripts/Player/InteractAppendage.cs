using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractAppendage : MonoBehaviour {

	public Interactable currentInteractable;
	Collider2D c;

	void Start() {
		this.gameObject.layer = LayerMask.NameToLayer(Layers.Interactables);
		c = GetComponent<Collider2D>();
	}

	void Update() {
		if (GlobalController.pc.IsGrounded() && !GlobalController.pc.inCutscene && GlobalController.pc.canInteract) {
			c.enabled = true;
		} else {
			c.enabled = false;
		}
	}

	void OnTriggerEnter2D(Collider2D otherCol) {
		Interactable i = otherCol.GetComponent<Interactable>();
		if (i != null) {
			if (currentInteractable != null) {
				currentInteractable.RemovePrompt();
			}
			currentInteractable = i;
			i.AddPrompt();
		}
	}
	
	void OnTriggerExit2D(Collider2D otherCol) {
		if (otherCol.GetComponent<Interactable>() == this.currentInteractable && this.currentInteractable != null) {
			otherCol.GetComponent<Interactable>().RemovePrompt();
			currentInteractable = null;
		}
	}
}
