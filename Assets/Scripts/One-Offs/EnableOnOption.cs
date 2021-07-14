using UnityEngine;

public class EnableOnOption : MonoBehaviour, IStateUpdateListener {
	public string optionName;

	void Start() {
		gameObject.SetActive(GameOptions.LoadBool(optionName));
	}

	public void OnStateUpdate() {
		Start();
	}
}
