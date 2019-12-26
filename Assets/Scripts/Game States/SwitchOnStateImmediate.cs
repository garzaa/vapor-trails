using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchOnStateImmediate : SwitchOnState {

	public bool waitsUntilInvisible = false;
	bool queuedReaction = false;
	public GameObject targetObject;

	public void ReactToStateChange() {
		if (waitsUntilInvisible) {
			Renderer targetRenderer = targetObject.GetComponent<Renderer>();
			if (targetRenderer != null && GetComponent<Renderer>().isVisible) {
			queuedReaction = true;
				return;
			}
			Awake();
		} else {
			Awake();
		}
	}

	void OnBecameInvisible() {
		if (queuedReaction) {
			Awake();
			queuedReaction = false;
		}
	}

	override protected void Awake() {
		if (enableOnState) {
			targetObject.SetActive(GlobalController.HasFlag(gameFlag));
		} else {
			targetObject.SetActive(!GlobalController.HasFlag(gameFlag));
		}
	}
}
