using UnityEngine;

public class CameraLookPoint : Activatable {
    public GameObject target;
    CinemachineInterface cmInterface;

    void OnEnable() {
        cmInterface = GameObject.FindObjectOfType<CinemachineInterface>();
    }

    override public void ActivateSwitch(bool b) {
        if (b) {
            cmInterface.LookAtPoint(target.transform);
        } else {
            cmInterface.StopLookingAtPoint(target.transform);
        }
    }
}
