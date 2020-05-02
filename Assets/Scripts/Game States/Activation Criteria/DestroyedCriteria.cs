using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyedCriteria : ActivationCriteria {

	public List<GameObject> destroyedCheck;

	int childrenLastFrame = 0;

	override protected void UpdateSatisfied() {
		foreach (GameObject g in destroyedCheck) {
			if (g != null) satisfied = false;
		}
		satisfied = true;
		base.UpdateSatisfied();
	}

	void FixedUpdate() {
		if (childrenLastFrame != transform.childCount || transform.childCount == 0) {
			UpdateSatisfied();
		}
		childrenLastFrame = transform.childCount;
	}
}
