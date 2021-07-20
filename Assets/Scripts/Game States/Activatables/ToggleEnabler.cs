using UnityEngine;
using System.Collections.Generic;

public class ToggleEnabler : Activatable {
    public List<GameObject> targets;

    override public void ActivateSwitch(bool b) {
        if (b) {
            foreach (GameObject g in targets) {
                foreach (PersistentEnabled p in g.GetComponentsInChildren<PersistentEnabled>()) {
                    p.UpdatePersistentState(!g.activeSelf);
                }
                g.SetActive(!g.activeSelf);
            }
        }
    }
}
