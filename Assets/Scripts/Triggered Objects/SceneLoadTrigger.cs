using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class SceneLoadTrigger : PlayerTriggeredObject {

	public SceneField sceneToLoad;
	public string beaconName;

	public override void OnPlayerEnter() {
		GlobalController.LoadScene(sceneToLoad, beaconName: beaconName);
	}

	public override void OnPlayerExit() {

	}
}
