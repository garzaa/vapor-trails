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
		SaveManager.LoadGame();
	}

	public void Exit() {
		Application.Quit();
	}

	public void Unpause() {
		GlobalController.Unpause();
	}

	public void ApplyGameOptions() {
		SaveManager.save.options.Apply();
	}

	public void LoadGameOptions() {
		SaveManager.save.options.Load();
	}
}
