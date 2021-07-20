using UnityEngine;

public class Disabler : Activatable {

    public GameObject target;

    override public void ActivateSwitch(bool b) {
        if (b) {
            target.SetActive(false);
            foreach (PersistentEnabled p in target.GetComponentsInChildren<PersistentEnabled>()) {
                p.UpdatePersistentState(false);
            }
        }
    }
}
