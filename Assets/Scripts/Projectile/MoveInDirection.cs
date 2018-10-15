using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveInDirection : MonoBehaviour {

	public int startSpeed;

	public void Go() {
		GetComponent<Rigidbody2D>().velocity = transform.right.normalized * startSpeed;
	}
}
