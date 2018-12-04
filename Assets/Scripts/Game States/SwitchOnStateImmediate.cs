using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchOnStateImmediate : SwitchOnState {

	public bool waitsUntilInvisible = false;
	public bool queuedReaction = false;
	bool visible = false;

	public void ReactToStateChange() {
		if (waitsUntilInvisible) {
			queuedReaction = true;
		} else {
			Awake();
		}
	}

	void OnBecameInvisible() {
		this.visible = false;
		if (queuedReaction) {
			Awake();
			queuedReaction = false;
		}
	}

	void OnBecameVisible() {
		this.visible = true;
	}

}
