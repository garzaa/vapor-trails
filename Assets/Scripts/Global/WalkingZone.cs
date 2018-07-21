using UnityEngine;

public class WalkingZone : PlayerTriggeredObject {

	public override void OnPlayerEnter() {
		player.ForceWalking();		
	}

	public override void OnPlayerExit() {
		player.StopForcedWalking();
	}
}
