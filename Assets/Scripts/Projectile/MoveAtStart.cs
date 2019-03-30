using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAtStart : MonoBehaviour {

	public Vector2 startVelocity;

	public void Go() {
		GetComponent<Rigidbody2D>().velocity = startVelocity;
	}
}
