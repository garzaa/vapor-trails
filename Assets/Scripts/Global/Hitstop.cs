using UnityEngine;
using System.Collections;

public class Hitstop : MonoBehaviour{

	public static Hitstop instance;

	static Coroutine currentHitstopRoutine;

	void Awake() {
		if (instance == null) instance = this;
	}

	public static void Run(float seconds) {
		Interrupt();
		Time.timeScale = 0.01f;
		currentHitstopRoutine = instance.StartCoroutine(EndHitstop(seconds));
	}

	static IEnumerator EndHitstop(float seconds) {
		yield return new WaitForSecondsRealtime(seconds);
		Time.timeScale = 1f;
	}

	public static void Interrupt() {
		if (currentHitstopRoutine != null) instance.StopCoroutine(currentHitstopRoutine);
		Time.timeScale = 1f;
	}
}