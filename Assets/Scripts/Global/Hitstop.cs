using UnityEngine;
using System.Collections;

public class Hitstop : MonoBehaviour {

	public static Hitstop instance;
	static Animator playerAnimator;

	static Coroutine currentHitstopRoutine;
	static bool currentPriority;

	void Awake() {
		if (instance == null) instance = this;
	}

	void Start() {
		playerAnimator = GlobalController.pc.GetComponent<Animator>();
	}

	public static void Run(float seconds, bool priority=false) {
		if (currentPriority && !priority) return;

		Interrupt();
		currentPriority = priority;
		Time.timeScale = 0.01f;
		playerAnimator.speed = 0;
		currentHitstopRoutine = instance.StartCoroutine(EndHitstop(seconds));
	}

	static IEnumerator EndHitstop(float seconds) {
		yield return new WaitForSecondsRealtime(seconds);
		currentPriority = false;
		playerAnimator.speed = 1f;
		Time.timeScale = 1f;
	}

	public static void Interrupt() {
		if (currentHitstopRoutine != null) instance.StopCoroutine(currentHitstopRoutine);
		if (playerAnimator != null) playerAnimator.speed = 1f;
		Time.timeScale = 1f;
		currentPriority = false;
	}
}