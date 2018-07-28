using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchOnState : MonoBehaviour {

	public string gameFlag;
	public bool enableOnState;

	void Start() {
		if (enableOnState) {
			this.gameObject.SetActive(GlobalController.HasFlag(gameFlag));
		} else {
			this.gameObject.SetActive(!GlobalController.HasFlag(gameFlag));
		}
	}
	
}
