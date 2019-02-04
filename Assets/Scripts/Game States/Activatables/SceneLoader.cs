using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class SceneLoader : Activatable {

	public SceneField sceneToLoad;

	public override void Activate() {
		GlobalController.LoadScene(sceneToLoad);
	}
}
