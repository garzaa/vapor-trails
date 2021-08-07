using UnityEngine;

public class EnableOnOption : StateChangeReactor {
	public string optionName;

	void Start() {
		gameObject.SetActive(GameOptions.LoadBool(optionName));
	}

	public override void React(bool fakeSceneLoad) {
		Start();
	}
}
