using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchOnState : MonoBehaviour {

	public GameFlag gameFlag;
	public bool enableOnState;

	protected virtual void Awake() {
		if (gameFlag == GameFlag.FoughtRichterOnce) {
			print(GlobalController.HasFlag(gameFlag).ToString());
		}
		if (enableOnState) {
			this.gameObject.SetActive(GlobalController.HasFlag(gameFlag));
		} else {
			this.gameObject.SetActive(!GlobalController.HasFlag(gameFlag));
		}
	}
	
}
