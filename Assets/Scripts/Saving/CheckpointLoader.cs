using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CheckpointLoader : PersistentObject {
	public GameCheckpoint checkpoint;

	protected override void SetDefaults() {
		if (!Application.isEditor) {
			return;
		}

		SetDefault("started", false);
		if (!GetProperty<bool>("started")) {
			StartCoroutine(Import());

			SetProperty("started", true);
		}
	}

	IEnumerator Import() {
		yield return new WaitForEndOfFrame();
		checkpoint.Import();
	}
}
