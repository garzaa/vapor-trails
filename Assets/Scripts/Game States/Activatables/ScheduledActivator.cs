using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScheduledActivator : Activator {

	float timeout = 0f;
	float delay = 0f;

	public override void Start() {
		base.Start();
		Invoke("InvokedActivation", delay);
	}

	void InvokedActivation() {
		Activate();
		Invoke("InvokedActivation", timeout);
	}
}
