using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSensor : Sensor {

	void Update() {
		Vector2 playerDist = player.transform.position - this.transform.position;
		if (pc.IsDead()) {
			playerDist = new Vector2(10000, 10000);
		}
		animator.SetFloat("PlayerXDist", Mathf.Abs(playerDist.x));
		animator.SetFloat("PlayerYDist", Mathf.Abs(playerDist.y));
	}
}
