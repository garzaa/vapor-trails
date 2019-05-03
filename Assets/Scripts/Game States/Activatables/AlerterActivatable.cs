using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AlerterActivatable : Activatable {
    public string[] alerts;

    override public void Activate() {
        AlerterText.AlertList(alerts);
    }

    override public void ActivateSwitch(bool b) {
        if (b) Activate();
    }
}