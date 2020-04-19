using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSensor : Sensor {

	public bool alsoForPlayer = false;

	void Update() {
		animator.SetInteger("HP", e.hp);
		if (alsoForPlayer) animator.SetInteger("PlayerHP", pc.currentHP);
	}
}
