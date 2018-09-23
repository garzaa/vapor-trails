using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : Interactable {

	public string sceneName;
	public string beaconName;

	public override void Interact(GameObject player) {
		GlobalController.LoadScene(sceneName, beaconName);
	}
}
