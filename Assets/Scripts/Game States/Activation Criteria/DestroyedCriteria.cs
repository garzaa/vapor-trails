using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyedCriteria : ActivationCriteria {

	public List<GameObject> destroyedCheck;

	public override bool CheckSatisfied() {
		foreach (GameObject g in destroyedCheck) {
			if (g != null) return false;
		}
		return true;
	}
}
