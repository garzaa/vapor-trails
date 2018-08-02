using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Activator : MonoBehaviour {

	public List<ActivationCriteria> criteria;
	public List<Activatable> objectsToActivate;

	public bool switchOnActivation = true;
	bool activated = false;

	public virtual void Start() {
		//if it's set to disable everything at the start, make sure it's enabled
		foreach (Activatable a in objectsToActivate) {
			if (!switchOnActivation) a.ActivateSwitch(true);
		}
	}

	void FixedUpdate() {
		if (activated) return;

		bool allSatisfied = true;
		foreach (ActivationCriteria c in criteria) {
			if (!c.CheckSatisfied()) {
				allSatisfied = false;
				break;
			}
		}
		if (allSatisfied) {
			activated = true;
			ActivateSwitch();
		}
	}

	public virtual void ActivateSwitch() {
		foreach (Activatable a in objectsToActivate) {
			if (a != null) a.ActivateSwitch(switchOnActivation);
		}
	}
}
