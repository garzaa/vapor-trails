using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchOnStateImmediate : SwitchOnState {

	public bool waitsUntilInvisible = false;
	bool queuedReaction = false;

	public void ReactToStateChange() {
		if (waitsUntilInvisible) {
			queuedReaction = true;
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
}
