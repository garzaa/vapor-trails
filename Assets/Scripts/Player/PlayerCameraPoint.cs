using UnityEngine;

public class PlayerCameraPoint : MonoBehaviour {
	float lookaheadRatio;
	Vector3 lookaheadDelta;

	PlayerController pc;
	const float stickMultiplier = 2f;
	const float xMultiplier = 2f;
	const float yMultiplier = 1.6f;
	Vector3 maxLookahead = new Vector3(4.3f, 2.1f);

	Vector3 targetPosition = Vector3.zero;
	Vector3 velocity = Vector3.zero;
	float smoothTime = 0.2f;
	float actualSmoothTime;

	bool disableLook = false;

	// this is nested as a child to have different smoothing parameters between movement and lookahead
	public Transform lookaheadTarget;
	Vector3 lookaheadTargetVelocity;
	float lookaheadMaxSpeed = 10f;
	float lookaheadSmoothTime = 0.5f;
	
	void Start() {
		pc = GameObject.FindObjectOfType<PlayerController>();
		transform.position = pc.transform.position;
		disableLook = TransitionManager.sceneData?.disableCameraLook ?? false;
	}

	public void OnOptionsClose() {
		// options slider is from 1 to 5
		this.lookaheadRatio = GlobalController.save.options.lookaheadRatio / 5f;
		actualSmoothTime = smoothTime * lookaheadRatio;
	}

	void LateUpdate() {
		UpdateBaseFollow();
		UpdateLookahead();
	}

	public void SnapToPlayer() {
		transform.position = pc.transform.position;
		velocity = Vector2.zero;
		lookaheadTarget.localPosition = GetLookaheadVector(true);
		lookaheadTargetVelocity = Vector2.zero;
	}

	void UpdateBaseFollow() {
		targetPosition = pc.transform.position;

		transform.position = Vector3.SmoothDamp(
			transform.position,
			pc.transform.position,
			currentVelocity: ref velocity,
			smoothTime: actualSmoothTime
		);
	}

	Vector2 GetLookaheadVector(bool isGrounded) {
		Vector2 v = Vector3.zero;
		v.x = pc.ForwardScalar() * pc.MoveSpeedRatio() * lookaheadRatio * xMultiplier;
		v.y = pc.IsGrounded() ? (1 * lookaheadRatio) : (pc.YMoveSpeedRatio() * lookaheadRatio * yMultiplier);

		v.x = Mathf.Clamp(v.x, -maxLookahead.x, maxLookahead.x);
		v.y = Mathf.Clamp(v.y, -maxLookahead.y, maxLookahead.y);

		// then right stick offset
		if (!disableLook) v += InputManager.RightStick() * stickMultiplier;
		return v;
	}

	void UpdateLookahead() {
		lookaheadDelta = GetLookaheadVector(pc.IsGrounded());

		lookaheadTarget.localPosition = Vector3.SmoothDamp(
			lookaheadTarget.localPosition,
			lookaheadDelta,
			currentVelocity: ref lookaheadTargetVelocity,
			smoothTime: lookaheadSmoothTime,
			maxSpeed: lookaheadMaxSpeed
		);
	}
}
