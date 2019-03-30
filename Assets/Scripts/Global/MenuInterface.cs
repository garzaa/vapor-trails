using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuInterface : MonoBehaviour {

	public GameObject continueGame;

	void Start() {
		continueGame.SetActive(GlobalController.HasSavedGame());
	}

	public void LoadScene(string sceneName) {
		GlobalController.LoadScene(sceneName);
	}
	
	public void LoadGame() {
		GlobalController.LoadGame();
	}

	public void Exit() {
		Application.Quit();
	}

	public void Unpause() {
		GlobalController.Unpause();
	}
}
