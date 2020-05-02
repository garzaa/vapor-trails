using UnityEngine;

public class CameraLookPoint : Activatable {
    public GameObject target;

    override public void ActivateSwitch(bool b) {
        if (b) {
            GlobalController.playerFollower.FollowTarget(target);
        } else {
            GlobalController.playerFollower.FollowPlayer();
        }
    }
}