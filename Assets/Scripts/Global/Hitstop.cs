using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitstop : MonoBehaviour{

	//statically calling instances
	public static Hitstop instance;

	void Awake() {
		if (instance == null) instance = this;
	}

	public static void Run(float seconds) {
		instance.StartCoroutine(DoHitstop(seconds));
	}

	static IEnumerator DoHitstop(float seconds) {
		Time.timeScale = 0.1f;
		yield return new WaitForSecondsRealtime(seconds);
		Time.timeScale = 1f;
	}
}