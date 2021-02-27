using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSensor : Sensor {

	public bool absoluteX = true;
	Vector2 playerDist;

	public bool running = false;

	void Update() {
		if (pc.IsDead() || !running) {
			playerDist = new Vector2(10000, 10000);
		} else {
			playerDist = player.transform.position - this.transform.position;
		}
		animator.SetFloat("PlayerXDist", absoluteX ? Mathf.Abs(playerDist.x) : playerDist.x);
		animator.SetFloat("PlayerYDist", Mathf.Abs(playerDist.y));
	}
}
