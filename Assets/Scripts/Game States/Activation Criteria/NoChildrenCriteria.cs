using UnityEngine;

public class NoChildrenCriteria : ActivationCriteria {

	public override bool CheckSatisfied() {
		return transform.childCount == 0;
    }
}
