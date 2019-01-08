using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventActivatable : Activatable {
	public UnityEvent eventToActivate;

	public override void ActivateSwitch(bool b) {
		if (b) {
			eventToActivate.Invoke();
		}
	}
}
