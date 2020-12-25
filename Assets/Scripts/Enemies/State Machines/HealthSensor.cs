using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSensor : Sensor {

	public bool alsoForPlayer = false;
	Enemy enemy;

	override protected void Start() {
		base.Start();
		enemy = e.GetComponent<Enemy>();
	}

	void Update() {
		animator.SetInteger("HP", enemy.hp);
		if (alsoForPlayer) animator.SetInteger("PlayerHP", pc.currentHP);
	}
}
