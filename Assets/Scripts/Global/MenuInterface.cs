using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuInterface : MonoBehaviour {

	public void LoadScene(string sceneName) {
		GlobalController.LoadScene(sceneName);
	}

	public void Exit() {
		Application.Quit();
	}

	public void Unpause() {
		GlobalController.Unpause();
	}
}
