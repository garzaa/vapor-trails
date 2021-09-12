using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentEnabled : PersistentObject {

	bool firstEnable = true;

	bool calledCorrectly;

	void OnEnable() {
		if (firstEnable) {
			firstEnable = false;

			SerializedPersistentObject o = LoadObjectState();
			if (o != null) {
				bool wasEnabled = (bool) o.persistentProperties["enabled"];
				if (!wasEnabled) {
					Disable();
				}
			}
		} else {
			UpdateState(true);
		}
	}

	public void UpdatePersistentState(bool active) {
		UpdateState(active);
	}

	void OnDisable() {
		if (!calledCorrectly && Application.isPlaying) {
			// this gets called when the application quits so kind of doesn't work (??maybe?? maybe my code's just bad)
			// Debug.LogWarning(gameObject.name + " has a persistentEnabled but was disabled normally!");
		}
	}

	// this is called from an item animation
	public void Disable() {
		calledCorrectly = true;
		UpdateState(false);
		gameObject.SetActive(false);
	}
	
	protected void UpdateState(bool e) {
		persistentProperties = new Hashtable();
		persistentProperties.Add("enabled", e);
		SaveObjectState();
	}
}
