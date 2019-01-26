using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestruct : MonoBehaviour {

	public float timer = 0f;
	public bool randRange;
	public float loBound;
	public float hiBound;

	Coroutine timeOut;

	void Start() {
		if (timer > 0 && !randRange) {
			WaitAndDestroy(timer);
		} else if (randRange) {
			WaitAndDestroy(Random.Range(loBound, hiBound));
		}
	}

	public void Destroy(float time = 0f) {
		timeOut = StartCoroutine(DestroyIn(time));
	}

	public void StopTimeout() {
		if (timeOut != null) {
			StopCoroutine(timeOut);
		}
	}

	//to call from other objects
	public void WaitAndDestroy(float time) {
		StopTimeout();
		timeOut = StartCoroutine(DestroyIn(time));
	}

	IEnumerator DestroyIn(float seconds) {
		print(gameObject.name);
		yield return new WaitForSeconds(seconds);
		Destroy(this.gameObject);
	}
}
