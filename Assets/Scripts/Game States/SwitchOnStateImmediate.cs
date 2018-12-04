using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchOnStateImmediate : SwitchOnState {

	public bool waitsUntilInvisible = false;
	public bool queuedReaction = false;
	bool visible = false;

	override protected void Awake() {
		base.Awake();
	}

	public void ReactToStateChange() {
		Awake();
	}

	void OnBecameInvisible() {
		this.visible = false;
		if (queuedReaction) {
			ReactToStateChange();
			queuedReaction = false;
		}
	}

	void OnBecameVisible() {
		this.visible = true;
	}

}
