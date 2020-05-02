using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEnterCriteria : ActivationCriteria {

	void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.CompareTag(Tags.Player)) {
			satisfied = true;
			UpdateSatisfied();
		}
	}

	void OnTriggerExit2D(Collider2D other) {
		if (other.gameObject.CompareTag(Tags.Player)) {
			satisfied = false;
			UpdateSatisfied();
		}
	}
	
}
