using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEnterCriteria : ActivationCriteria {

	bool containsPlayer = false;

	void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.CompareTag(Tags.Player)) {
			this.containsPlayer = true;
		}
	}

	void OnTriggerExit2D(Collider2D other) {
		if (other.gameObject.CompareTag(Tags.Player)) {
			this.containsPlayer = true;
		}
	}

	override public bool CheckSatisfied() {
		return containsPlayer;
	}
	
}
