using UnityEngine;

public class MetroPassListener : MonoBehaviour {
	bool gotMetroPass = false;

	public void Check() {
		if (gotMetroPass)
	}

	public void OnItemGet(String itemName) {
		if (itemName == "Metro Pass") {
			gotMetroPass = true;
		}
	}
}
