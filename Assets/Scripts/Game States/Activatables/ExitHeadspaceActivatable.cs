using UnityEngine;

public class ExitHeadspaceActivatable : Activatable {
    
    public override void Activate() {
        GlobalController.gc.GetComponent<HeadspaceController>().LeaveHeadspace();
    }

    public override void ActivateSwitch(bool b) {
        if (b) Activate();
    }
}