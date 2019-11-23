using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class SceneLoadTrigger : PlayerTriggeredObject {

	public SceneField sceneToLoad;
	public Beacon beacon;

	override protected void Start() {
		base.Start();
		Instantiate(Resources.Load("DoorIcon"), transform.position, Quaternion.identity, this.transform);
	}

	public override void OnPlayerEnter() {
		GlobalController.LoadScene(sceneToLoad, beacon: beacon);
	}

	public override void OnPlayerExit() {

	}
}
