using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoadTrigger : PlayerTriggeredObject {

	public string sceneName;

	public override void OnPlayerEnter() {
		GlobalController.LoadScene(sceneName);
	}

	public override void OnPlayerExit() {

	}
}
