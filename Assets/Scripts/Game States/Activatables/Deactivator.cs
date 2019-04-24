using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deactivator : Activatable {

    public Activatable target;

    override public void Activate() {
        target.ActivateSwitch(false);
    }

    override public void ActivateSwitch(bool b) {
        if (b) {
            target.ActivateSwitch(false);
        } else {
            target.ActivateSwitch(true);
        }
    }

}
