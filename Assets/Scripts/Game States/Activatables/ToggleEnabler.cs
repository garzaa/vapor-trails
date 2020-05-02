using UnityEngine;
using System.Collections.Generic;

public class ToggleEnabler : Activatable {
    public List<GameObject> targets;

    override public void ActivateSwitch(bool b) {
        if (b) {
            foreach (GameObject g in targets) {
                g.SetActive(!g.activeSelf);
            }
        }
    }
}