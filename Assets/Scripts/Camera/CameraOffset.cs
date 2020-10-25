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

	public bool stickOffset;
	float stickOffsetMultiplier = .05f;
	float speedRamp = 2f;

	public bool clampPosition;
	public Vector2 maxLookahead;

	GameObject otherTarget;

	void Start() {
		pc = player.GetComponent<PlayerController>();
	}

	void FixedUpdate() {
		if (!following) {
			transform.localPosition = Vector3.zero;
			return;
		}

		Vector3 newPosition = player.transform.position;

		if (lookingAhead && otherTarget==null) {
			//first offset based on player orientation
			float newX = pc.ForwardScalar() * pc.MoveSpeedRatio() * lookAhead * speedRamp;
			float scalar = pc.IsGrounded() ? 1 : 0;
			float newY = scalar * lookUp;

			// but actually no
			if (clampPosition) {
					newX = Mathf.Clamp(newX, -maxLookahead.x, maxLookahead.x);
					newY = Mathf.Clamp(newY, -maxLookahead.y, maxLookahead.y);
			}
			
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

		if (otherTarget != null) {
			newPosition += (otherTarget.transform.position - this.transform.position);
		}

		//then update camera pos
		transform.position = Vector3.SmoothDamp(
			transform.position,
			newPosition,
			ref velocity,
			smoothAmount * Time.deltaTime
		);

		if (stickOffset) transform.position += (Vector3) InputManager.RightStick() * stickOffsetMultiplier; 
	}

	public void SetOffset(Vector2 newOffset) {
		this.offset = newOffset;
	}

	public void OnBossFightStart() {
		LookAtTarget(GameObject.FindObjectOfType<Boss>().gameObject);
	}

	public void OnBossFightStop() {
		StopLookingAtTarget();
	}

	public void LookAtTarget(GameObject point) {
		otherTarget = point;
	}

	public void StopLookingAtTarget() {
		otherTarget = null;
	}
}
