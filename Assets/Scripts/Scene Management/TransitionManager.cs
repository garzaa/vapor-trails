using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionManager : MonoBehaviour {

	void Start() {
		OnSceneLoaded();
	}

	void OnSceneLoaded() {
		SceneData sd;
		if (GameObject.Find("SceneData") != null) {
			sd = GameObject.Find("SceneData").GetComponent<SceneData>();
			GlobalController.ShowTitleText(sd.title, sd.subTitle);
		}
		
	}
	
}
