using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOffset : MonoBehaviour {

	Vector2 offset;
	Vector3 velocity = Vector3.zero;
	public GameObject player;
	Entity pc;
	public bool lookingAhead = true;
	public bool offsetEnabled = true;
	
	public float smoothAmount;
	public float lookAhead;

	void Start() {
		pc = player.GetComponent<PlayerController>();
	}

	void FixedUpdate() {
		Vector3 newPosition = player.transform.position;

		if (lookingAhead) {
			//first offset based on player orientation
			newPosition = new Vector3(
				newPosition.x + pc.GetForwardScalar() * lookAhead,
				newPosition.y,
				newPosition.z	
			);
		}

		//then any other offset
		if (offsetEnabled) {
			newPosition += (Vector3) offset;
		}

		//then update movement
		transform.position = Vector3.SmoothDamp(
			transform.position,
			newPosition,
			ref velocity,
			smoothAmount * Time.deltaTime
		);
		Debug.Log(pc.GetForwardScalar() * lookAhead);
	}

	public void SetOffset(Vector2 newOffset) {
		this.offset = newOffset;
	}
}
