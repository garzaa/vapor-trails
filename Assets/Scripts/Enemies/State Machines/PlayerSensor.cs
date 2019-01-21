using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSensor : Sensor {

	void Update() {
		Vector2 playerDist = player.transform.position - this.transform.position;
		animator.SetFloat("PlayerXDist", Mathf.Abs(playerDist.x));
		animator.SetFloat("PlayerYDist", playerDist.y);
	}
}
