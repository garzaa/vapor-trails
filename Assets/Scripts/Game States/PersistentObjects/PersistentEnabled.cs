using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentEnabled : PersistentObject {

	bool firstEnable = true;

	void OnEnable() {
		if (firstEnable) {
			firstEnable = false;

			SerializedPersistentObject o = LoadObjectState();
			if (o != null) {
				bool wasEnabled = (bool) o.persistentProperties["enabled"];
				if (!wasEnabled) {
					UpdateState(false);
					gameObject.SetActive(false);
				}
			}
		} else {
			UpdateState(true);
		}
	}

	public void UpdatePersistentState(bool active) {
		UpdateState(active);
	}

	// this is called from an item animation
	public void Disable() {
		UpdateState(false);
		gameObject.SetActive(false);
	}
	
	protected void UpdateState(bool e) {
		persistentProperties = new Hashtable();
		persistentProperties.Add("enabled", e);
		SaveObjectState();
	}
}
