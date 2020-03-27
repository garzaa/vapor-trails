using UnityEngine;

public class NoChildrenCriteria : ActivationCriteria {

	public GameObject optionalContainer;

	override protected void UpdateSatisfied() {
		if (optionalContainer != null) {
			satisfied = optionalContainer.transform.childCount == 0;
		} else {
			satisfied = transform.childCount == 0;
		}
		base.UpdateSatisfied();
    }

	void FixedUpdate() {
		UpdateSatisfied();
	}
}
