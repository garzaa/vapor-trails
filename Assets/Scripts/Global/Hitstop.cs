using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitstop : MonoBehaviour{

	//statically calling instances
	public static Hitstop instance;

	void Awake() {
		instance = this;
	}

	public static void Run(float seconds) {
		Time.timeScale = 0.1f;
		instance.CancelInvoke("EndHitstop");
		instance.Invoke("EndHitstop", seconds);
	}

	static void EndHitstop() {
		Time.timeScale = 0.1f;
	}
}