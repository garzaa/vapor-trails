using UnityEngine;

public class WalkingZone : PlayerTriggeredObject {

	public override void OnPlayerEnter() {
		if (player != null)
			player.ForceWalking();		
	}

	public override void OnPlayerExit() {
		player.StopForcedWalking();
	}
}
