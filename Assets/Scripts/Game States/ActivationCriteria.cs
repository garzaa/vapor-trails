using UnityEngine;

public abstract class ActivationCriteria : MonoBehaviour {

	public bool satisfied;
	Activator activator;

	private bool satisfiedLastCheck;

	protected virtual void UpdateSatisfied() {
		if (!satisfiedLastCheck && satisfied) {
			activator.CheckCriteria();
		}
		satisfiedLastCheck = satisfied;
	}

	public void OnRegister(Activator activator) {
		this.activator = activator;
		UpdateSatisfied();
	}
}
