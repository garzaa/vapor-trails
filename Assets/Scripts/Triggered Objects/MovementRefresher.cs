using UnityEngine;

public class MovementRefresher : PlayerTriggeredObject {
    public override void OnPlayerEnter() {
        this.player.RefreshAirMovement();
    }

    public override void OnPlayerExit() {
        
    }
}
