using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentEnabled : PersistentObject {

	void OnEnable() {
		SerializedPersistentObject o = LoadObjectState();
		if (o != null) {
			if (!((bool) o.persistentProperties["enabled"])) {
				Disable();
				return;
			}
		}
		UpdateState(true);
	}

	//this NEEDS to be called to update the state
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
