using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities;

public class Door : Interactable {

	public SceneField sceneToLoad;
	public string beaconName;

	public override void Interact(GameObject player) {
		GlobalController.LoadScene(sceneToLoad, beaconName);
	}
}
