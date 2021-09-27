using UnityEngine;

public class DayWatcher : MonoBehaviour {
	void OnEnable() {
		GameObject.FindObjectOfType<Daytime>().Register(this);
	}

	void OnDisable() {
		GameObject.FindObjectOfType<Daytime>().Deregister(this);
	}

	abstract public void OnDayUpdate(float t);
}
