using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFlagOnActivate : Activatable {
	public GameFlag flagName;

	public override void ActivateSwitch(bool b) {
		if (b) GlobalController.AddGameFlag(this.flagName);
	}
}
