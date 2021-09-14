using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuInterface : MonoBehaviour {

	public GameObject continueGame;

	void Start() {
		if (continueGame != null) {
			continueGame.SetActive(GlobalController.HasSavedGame());
		}
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

	public void ApplyGameOptions() {
		GlobalController.save.options.Apply();
	}

	public void LoadGameOptions() {
		GlobalController.save.options.Load();
	}
}
