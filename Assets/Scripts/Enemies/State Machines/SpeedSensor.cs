using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedSensor : Sensor {

	Rigidbody2D rb2d;

	new void Start() {
		base.Start();
		if (e != null) {
			rb2d = e.GetComponent<Rigidbody2D>();
		} else {
			rb2d = animator.GetComponent<Rigidbody2D>();
		}
	}

	void Update () {
		animator.SetFloat("SpeedX", Mathf.Abs(rb2d.velocity.x));
		animator.SetFloat("SpeedY", Mathf.Abs(rb2d.velocity.y));
	}
}
