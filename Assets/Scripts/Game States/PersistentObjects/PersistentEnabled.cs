using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentEnabled : PersistentObject {

	bool firstEnable = true;

	void OnEnable() {
		if (firstEnable) {
			firstEnable = false;
			return;
		} else {
			SerializedPersistentObject o = LoadObjectState();
			if (o != null) {
				bool enabled = (bool) o.persistentProperties["enabled"];
				this.gameObject.SetActive(enabled);
				UpdateObjectState();
			}
		}
	}

	void OnDisable() {
		UpdateObjectState();
	}

	public override void Start() {
		OnEnable();
	}

	protected override void UpdateObjectState() {
		persistentProperties = new Hashtable();
		persistentProperties.Add("enabled", this.enabled);
		SaveObjectState();
	}
	
}
