using UnityEngine;

public class ActivatedAlertText : Activatable {

    public string[] alerts;

    override public void Activate() {
        AlerterText.AlertList(this.alerts);
    }

    override public void ActivateSwitch(bool b) {
        if (b) Activate();
    }
}