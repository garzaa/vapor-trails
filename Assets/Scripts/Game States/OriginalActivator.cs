using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OriginalActivator : MonoBehaviour {

	public List<ActivationCriteria> criteria;
	public List<Activatable> objectsToActivate;

	public bool switchOnActivation = true;
	bool activated = false;
	public bool singleActivation = true;

	public virtual void Start() {
		//if it's set to disable everything at the start, make sure it's enabled
		foreach (Activatable a in objectsToActivate) {
			if (!switchOnActivation) a.ActivateSwitch(true);
		}
	}

	void FixedUpdate() {
		if (singleActivation && activated) return;

		bool allSatisfied = false;
		if (criteria.Count > 0) {
			allSatisfied = true;
			foreach (ActivationCriteria c in criteria) {
				if (!c.CheckSatisfied()) {
					allSatisfied = false;
					break;
				}
			}
		}

		if (allSatisfied) {
			activated = true;
			Activate();
		}
	}

	public virtual void Activate() {
		foreach (Activatable a in objectsToActivate) {
			if (a != null) a.ActivateSwitch(switchOnActivation);
		}
	}
}
