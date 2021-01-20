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
	public float lookaheadRatio = 1;

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

	public void ReactToOptionsClose() {
		GameOptions options = GlobalController.save.options;
		this.lookaheadRatio = options.lookaheadRatio;
	}

	void LateUpdate() {
		if (!following) {
			transform.localPosition = Vector3.zero;
			return;
		}

		Vector3 newPosition = player.transform.position;

		if (lookingAhead && otherTarget==null) {
			Vector3 lookaheadDelta = new Vector3();

			//first offset based on player orientation
			// look based on speed, if speed 0 then based on orientation
			float forwardScalar = pc.MoveSpeedRatio() == 0 ? pc.ForwardScalar() : (pc.movingRight ? 1 : -1);
			lookaheadDelta.x = forwardScalar * pc.MoveSpeedRatio() * lookAhead * speedRamp;
			float scalar = pc.IsGrounded() ? 1 : 0;
			lookaheadDelta.y = scalar * lookUp;

			// but actually no
			if (clampPosition) {
					lookaheadDelta.x = Mathf.Clamp(lookaheadDelta.x, -maxLookahead.x, maxLookahead.x);
					lookaheadDelta.y = Mathf.Clamp(lookaheadDelta.y, -maxLookahead.y, maxLookahead.y);
			}

			// lookahead ratio in-game is a slider from 0-5
			lookaheadDelta.x *= (lookaheadRatio/5f);
			
			newPosition += lookaheadDelta;
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
			smoothAmount * Time.deltaTime,
			maxSpeed: 10
		);

		if (stickOffset) transform.position += (Vector3) InputManager.RightStick() * stickOffsetMultiplier; 
	}

	public void SetOffset(Vector2 newOffset) {
		this.offset = newOffset;
	}

	public void OnBossFightStart() {
		Boss b = GameObject.FindObjectOfType<Boss>();
		LookAtTarget((b.cameraAnchor != null) ? b.cameraAnchor : b.gameObject);
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
