using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraGrabber : PlayerTriggeredObject {

	public GameObject targetPoint;

	public override void OnPlayerEnter() {
		if (targetPoint != null) {
			GlobalController.playerFollower.FollowTarget(targetPoint);
		} else {
			GlobalController.playerFollower.DisableFollowing();
		}
	}

	public override void OnPlayerExit() {
		GlobalController.playerFollower.EnableFollowing();
		GlobalController.playerFollower.FollowPlayer();
	}
}
