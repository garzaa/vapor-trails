using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistentEnabled : PersistentObject {

	bool firstEnable = true;

	bool calledCorrectly;

	protected override void SetDefaults() {
		if (firstEnable) {
			firstEnable = false;
			SetDefault("enabled", true);
			if (!GetProperty<bool>("enabled")) {
				Disable();
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
		calledCorrectly = true;
		UpdateState(false);
		gameObject.SetActive(false);
	}
	
	protected void UpdateState(bool e) {
		SetProperty("enabled", e);
	}
}
