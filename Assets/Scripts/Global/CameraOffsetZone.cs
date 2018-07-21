using UnityEngine;

public class CameraOffsetZone : PlayerTriggeredObject {

	public Vector2 offset;

	public override void OnPlayerEnter() {
		GlobalController.playerFollower.UpdateOffset(this.offset);
	}

	public override void OnPlayerExit() {
		GlobalController.playerFollower.ResetOffset();
	}
}
