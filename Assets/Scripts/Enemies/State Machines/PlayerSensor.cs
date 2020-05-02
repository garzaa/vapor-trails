using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSensor : Sensor {

	public bool absoluteX = true;

	void Update() {
		Vector2 playerDist = player.transform.position - this.transform.position;
		if (pc.IsDead()) {
			playerDist = new Vector2(10000, 10000);
		}
		animator.SetFloat("PlayerXDist", absoluteX ? Mathf.Abs(playerDist.x) : playerDist.x);
		animator.SetFloat("PlayerYDist", Mathf.Abs(playerDist.y));
	}
}
