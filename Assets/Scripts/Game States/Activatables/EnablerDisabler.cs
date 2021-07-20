using UnityEngine;

public class EnablerDisabler : Activatable {

	public GameObject target;

	public override void ActivateSwitch(bool b) {
		foreach (PersistentEnabled p in target.GetComponentsInChildren<PersistentEnabled>()) {
			p.UpdatePersistentState(!target.activeSelf);
		}
		target.SetActive(!target.activeSelf);
	}

	public void Disable() {
		target.SetActive(false);
		foreach (PersistentEnabled p in target.GetComponentsInChildren<PersistentEnabled>()) {
			p.UpdatePersistentState(false);
		}
	}
}
