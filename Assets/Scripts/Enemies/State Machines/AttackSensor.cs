using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSensor : Sensor {

	void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.CompareTag(Tags.PlayerHitbox)) {
			animator.SetTrigger("AttackDetected");
		}
	}
	
}
