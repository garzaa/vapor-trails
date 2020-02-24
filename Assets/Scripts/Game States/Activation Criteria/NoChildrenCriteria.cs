using UnityEngine;

public class NoChildrenCriteria : ActivationCriteria {

	public GameObject optionalContainer;

	public override bool CheckSatisfied() {
		if (optionalContainer != null) return optionalContainer.transform.childCount == 0;
		return transform.childCount == 0;
    }
}
