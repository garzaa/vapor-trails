using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSensor : Sensor {

	void Update() {
		animator.SetInteger("HP", e.hp);
	}
}
