using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScheduledActivator : Activator {

	public float timeout = 0f;
	public float delay = 0f;

	[Header("Random Delay")]
	public bool randomDelayEnabled;
	public float lowBound;
	public float highBound;

	public override void Start() {
		base.Start();
		Invoke("InvokedActivation", delay);
	}

	void InvokedActivation() {
		Activate();
		Invoke("InvokedActivation", timeout);
	}

	float GetDelay() {
		return randomDelayEnabled ? Random.Range(lowBound, highBound) : delay;
	}
}
