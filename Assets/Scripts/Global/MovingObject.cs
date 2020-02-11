using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObject : MonoBehaviour {

	public GameObject mainObject;
	public GameObject pointA;
	public GameObject pointB;
	public float moveSpeed = 1;

	private bool returning = false;

	void Start () {
		if (pointA == null || pointB == null) {
			Debug.LogError("You forgot to assign the movement endpoints!");
		}
		mainObject.transform.position = pointA.transform.position;
	}

	//moves from point A to point B and then back	
	void FixedUpdate () {
		Move();
		if (!returning) {
			if (mainObject.transform.position == pointB.transform.position) {
				returning = true;
			}
		} else {
			if (mainObject.transform.position == pointA.transform.position) {
				returning = false;
			}
		}
	}

	void Move() {
		Vector3 target = returning ? pointA.transform.position : pointB.transform.position;
		float step = moveSpeed * Time.deltaTime;
        mainObject.transform.position = Vector3.MoveTowards(mainObject.transform.position, target, step);
	}
}
