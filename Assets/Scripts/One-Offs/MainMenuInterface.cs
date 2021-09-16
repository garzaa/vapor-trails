using UnityEngine;

public class MainMenuInterface : MonoBehaviour {
	public GameObject chapters;
	bool debug = false;

	void Start() {
		chapters.SetActive(false);
	}

	void Update() {
		if (Input.GetKeyDown(KeyCode.K)) {
			if (!debug) {
				debug = true;
				AlerterText.Alert("DEBUG MODE ENABLED");
				AlerterText.Alert("have fun homeboy");
			}
			chapters.SetActive(!chapters.activeInHierarchy);
		}
	}

}
