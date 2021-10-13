using UnityEngine;

[ExecuteInEditMode]
public class DayWatcher : MonoBehaviour {
	Daytime daytime;

	void OnEnable() {
		daytime = GameObject.FindObjectOfType<Daytime>();
		daytime.Register(this);
	}

	void OnDisable() {
		if (daytime) daytime.Deregister(this);
	}

	virtual public void OnDayUpdate(float t) {
		throw new System.NotImplementedException();
	}
}
