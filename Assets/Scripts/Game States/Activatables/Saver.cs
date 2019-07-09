﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Saver : Activatable {
	override public void Activate() {
		GlobalController.SaveGame(true);
	}

	override public void ActivateSwitch(bool b) {
		if (b) Activate();
	}
}
