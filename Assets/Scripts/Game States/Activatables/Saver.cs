using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Saver : Activatable {
	public override void Activate() {
		GlobalController.SaveGame(true);
	}
}
