using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class SceneLoadTrigger : PlayerTriggeredObject {

	public Beacon beacon;
	[Header("DEPRECATED")]
	public SceneField sceneToLoad;

	override protected void Start() {
		base.Start();
		Instantiate(Resources.Load("DoorIcon"), transform.position, Quaternion.identity, this.transform);
	}

	public override void OnPlayerEnter() {
		// don't fire multiple times
		this.enabled = false;
		GlobalController.LoadScene(sceneToLoad, beacon: beacon);
	}

	public override void OnPlayerExit() {

	}
}
