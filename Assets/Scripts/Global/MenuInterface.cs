using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuInterface : MonoBehaviour {

	public GameObject continueGame;
	public GameObject ngPlus;

	void Start() {
		continueGame.SetActive(GlobalController.HasSavedGame());
		ngPlus.SetActive((GlobalController.HasBeatGame()));
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

	public void NewGamePlus() {
		GlobalController.NewGamePlus();
	}
}
