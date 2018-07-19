using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiposteTrigger : MonoBehaviour {

	public bool armed = false;

	void OnTriggerEnter2D(Collider2D col) {
		if (col.GetComponent<EnemyAttack>()) {
			if (armed) {
				transform.parent.gameObject.GetComponent<PlayerController>().riposteTriggered = true;
				armed = false;
			}
		}
	}
	
}
