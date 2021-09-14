using UnityEngine;

public class MainMenuInterface : MonoBehaviour {
	public GameObject chapters;

	void Start() {
		chapters.SetActive(false);
	}

	void Update() {
		if (Input.GetKeyDown(KeyCode.K)) {
			if (!chapters) print("ad;kljhdsaf;lkjsdhf");
			chapters.SetActive(!chapters.activeInHierarchy);
		}
	}

}
