using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestruct : MonoBehaviour {

	public float timer = 0f;
	public bool randRange;
	public float loBound;
	public float hiBound;

	void Start() {
		if (timer > 0 && !randRange) {
			StartCoroutine(WaitAndDestroy(timer));
		} else if (randRange) {
			StartCoroutine(WaitAndDestroy(Random.Range(loBound, hiBound)));
		}
	}

	public void Destroy() {
		Destroy(this.gameObject);
	}

	IEnumerator WaitAndDestroy(float time) {
		yield return new WaitForSeconds(time);
		Destroy();
	}
}
