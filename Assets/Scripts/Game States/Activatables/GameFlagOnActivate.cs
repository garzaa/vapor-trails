using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFlagOnActivate : Activatable {
	//TODO: also make this an enum
	public GameFlag flagName;

	public override void ActivateSwitch(bool b) {
		if (b) GlobalController.AddGameFlag(this.flagName);
	}
}
