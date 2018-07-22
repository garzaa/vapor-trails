using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoadTrigger : PlayerTriggeredObject {

	public string sceneName;
	public string beaconName;

	public override void OnPlayerEnter() {
		GlobalController.LoadScene(sceneName, beaconName: beaconName);
	}

	public override void OnPlayerExit() {

	}
}
