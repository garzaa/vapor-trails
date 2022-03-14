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
	public float smoothAmount = 0.2f;
	public float actualSmoothAmount;

	bool disableLook = false;
	
	void Start() {
		pc = GameObject.FindObjectOfType<PlayerController>();
		transform.position = pc.transform.position;
		disableLook = TransitionManager.sceneData?.disableCameraLook ?? false;
	}

	public void OnOptionsClose() {
		// options slider is from 1 to 5
		this.lookaheadRatio = GlobalController.save.options.lookaheadRatio / 5f;
		actualSmoothAmount = smoothAmount * lookaheadRatio;
	}

	void LateUpdate() {
		lookaheadDelta = Vector3.zero;

		lookaheadDelta.x = pc.ForwardScalar() * pc.MoveSpeedRatio() * lookaheadRatio * xMultiplier;
		lookaheadDelta.y = pc.IsGrounded() ? (1 * lookaheadRatio) : (pc.YMoveSpeedRatio() * lookaheadRatio * yMultiplier);

		lookaheadDelta.x = Mathf.Clamp(lookaheadDelta.x, -maxLookahead.x, maxLookahead.x);
		lookaheadDelta.y = Mathf.Clamp(lookaheadDelta.y, -maxLookahead.y, maxLookahead.y);

		// then right stick offset
		if (!disableLook) lookaheadDelta += (Vector3) InputManager.RightStick() * stickMultiplier;

		targetPosition = pc.transform.position + lookaheadDelta;

		transform.position = Vector3.SmoothDamp(
			transform.position,
			targetPosition,
			ref velocity,
			actualSmoothAmount
		);
	}
}
