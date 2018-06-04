using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractAppendage : MonoBehaviour {

	public Interactable currentInteractable;

	void Start() {
		this.gameObject.layer = LayerMask.NameToLayer(Layers.Interactables);
	}

	void OnTriggerEnter2D(Collider2D otherCol) {
		Interactable i = otherCol.GetComponent<Interactable>();
		if (i != null) {
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
