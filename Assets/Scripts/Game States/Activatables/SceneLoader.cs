using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class SceneLoader : Activatable {

	public Beacon beacon;

    public override void ActivateSwitch(bool b) {
		if (b) {
			GlobalController.LoadScene(null, beacon);
		}
    }
}
