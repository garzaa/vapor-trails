using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiActivatable : Activatable {
	public List<Activatable> activatables;

	public override void ActivateSwitch(bool b) {
		foreach(Activatable a in activatables) {
			a.gameObject.SetActive(b);
		}
	}
}
