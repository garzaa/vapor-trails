using UnityEngine;

public class EnablerDisabler : Activatable {

	public GameObject target;

	public override void ActivateSwitch(bool b) {
		target.SetActive(!target.activeSelf);
	}

	public void Disable() {
		this.enabled = false;
	}
}
