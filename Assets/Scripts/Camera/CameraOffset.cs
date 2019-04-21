using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOffset : MonoBehaviour {

	Vector2 offset;
	Vector3 velocity = Vector3.zero;
	public GameObject player;
	PlayerController pc;
	public bool lookingAhead = true;
	public bool offsetEnabled = true;
	
	public float smoothAmount;
	public float lookAhead;
	public float lookUp;

	public bool following = true;

	void Start() {
		pc = player.GetComponent<PlayerController>();
	}

	void FixedUpdate() {
		if (!following) {
			transform.localPosition = Vector3.zero;
			return;
		}

		Vector3 newPosition = player.transform.position;

		if (lookingAhead) {
			//first offset based on player orientation
			float newX = pc.ForwardScalar() * pc.MoveSpeedRatio() * lookAhead;
			float scalar = pc.IsGrounded() ? 1 : 0;
			float newY = scalar * lookUp;
			newPosition = new Vector3(
				newPosition.x + newX,
				newPosition.y + newY,
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
	}

	public void SetOffset(Vector2 newOffset) {
		this.offset = newOffset;
	}
}
