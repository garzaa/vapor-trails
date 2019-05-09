using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Saver : Activatable {
	public override void ActivateSwitch(bool b) {
		GlobalController.SaveGame(true);
	}
}
