using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchOnStateImmediate : SwitchOnState {

	public bool waitsUntilInvisible = false;
	bool queuedReaction = false;
	public GameObject targetObject;

	void OnBecameInvisible() {
		if (queuedReaction) {
			UpdateSelf();
			queuedReaction = false;
		}
	}

	void UpdateSelf() {
		if (enableOnState) {
			targetObject.SetActive(GlobalController.HasFlag(gameFlag));
		} else {
			targetObject.SetActive(!GlobalController.HasFlag(gameFlag));
		}
	}

	public override void React(bool fakeSceneLoad) {
		if (waitsUntilInvisible) {
			Renderer targetRenderer = targetObject.GetComponent<Renderer>();
			if (targetRenderer != null && GetComponent<Renderer>().isVisible) {
			queuedReaction = true;
				return;
			}
			UpdateSelf();
		} else {
			UpdateSelf();
		}
	}
}
