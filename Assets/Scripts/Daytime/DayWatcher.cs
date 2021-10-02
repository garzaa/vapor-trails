using UnityEngine;

public class DayWatcher : MonoBehaviour {
	void OnEnable() {
		GameObject.FindObjectOfType<Daytime>().Register(this);
	}

	void OnDisable() {
		GameObject.FindObjectOfType<Daytime>().Deregister(this);
	}

	virtual public void OnDayUpdate(float t) {
		throw new System.NotImplementedException();
	}
}
