using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Daytime : MonoBehaviour {
	[Range(0, 1)]
	public float time;

	float lastTime;

	List<DayWatcher> dayWatchers = new List<DayWatcher>();

	public void Register(DayWatcher watcher) {
		dayWatchers.Add(watcher);
	}

	public void Deregister(DayWatcher watcher) {
		dayWatchers.Remove(watcher);
	}

	void UpdateWatchers() {
		foreach (DayWatcher w in dayWatchers) {
			w.OnDayUpdate(time);
		}
	}

	void Update() {
		if (time != lastTime) {
			UpdateWatchers();
		}

		lastTime = time;
	}
}
