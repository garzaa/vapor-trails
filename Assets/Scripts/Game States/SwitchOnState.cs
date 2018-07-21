using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchOnState : MonoBehaviour {

	string gameFlag;
	bool enableOnState;

	void Start() {
		if (enableOnState) {
			this.enabled = GlobalController.HasFlag(gameFlag);
		} else {
			this.enabled = !GlobalController.HasFlag(gameFlag);
		}
	}
	
}
