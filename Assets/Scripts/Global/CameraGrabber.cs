using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraGrabber : PlayerTriggeredObject {

	

	public override void OnPlayerEnter() {
		GlobalController.playerFollower.DisableFollowing();
	}

	public override void OnPlayerExit() {
		GlobalController.playerFollower.EnableFollowing();
	}
}
