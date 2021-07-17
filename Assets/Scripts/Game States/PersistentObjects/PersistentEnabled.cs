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
					Disable();
				}
			}
		} else {
			UpdateState(true);
		}
	}

	void OnDisable() {
		if (this.isActiveAndEnabled) {
			// this.isActiveAndEnabled is true when the object is disabled to be destroyed on a scene load
			// https://answers.unity.com/questions/882428/ondisable-getting-called-from-destroy.html
			return;
		}
		UpdateState(false);
	}

	public void Disable() {
		UpdateState(false);
		this.gameObject.SetActive(false);
	}
	
	protected void UpdateState(bool e) {
		persistentProperties = new Hashtable();
		persistentProperties.Add("enabled", e);
		SaveObjectState();
	}
}
