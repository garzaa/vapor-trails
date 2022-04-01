using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Saver : Activatable {

	public bool autosave = true;

	override public void Activate() {
		SaveManager.SaveGame(autosave: autosave);
	}

	override public void ActivateSwitch(bool b) {
		if (b) Activate();
	}
}
