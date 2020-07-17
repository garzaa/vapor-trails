using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities;

public class Door : Interactable {

	public SceneField sceneToLoad;
	public Beacon beacon;

	override protected void ExtendedStart() {
		Instantiate(Resources.Load("DoorIcon"), transform.position, Quaternion.identity, this.transform);
		if (beacon == null) Debug.LogWarning("Door "+gameObject.GetHierarchicalName()+" has no target beacon!");
	}

	public override void Interact(GameObject player) {
		base.Interact(player);
		GlobalController.LoadScene(sceneToLoad, beacon);
	}
}
