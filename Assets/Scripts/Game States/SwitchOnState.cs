using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchOnState : MonoBehaviour {

	public GameFlag gameFlag;
	public bool enableOnState;

	virtual protected void Awake() {
		if (enableOnState) {
			this.gameObject.SetActive(GlobalController.HasFlag(gameFlag));
		} else {
			this.gameObject.SetActive(!GlobalController.HasFlag(gameFlag));
		}
	}
	
}
