using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayActivatable : Activatable {
    public float delay;
	public List<Activatable> activatables;

	public override void ActivateSwitch(bool b) {
		StartCoroutine(WaitAndActivate(b));
	}

    IEnumerator WaitAndActivate(bool b) {
        yield return new WaitForSeconds(delay);
        foreach(Activatable a in activatables) {
			if (a != null)
			a.ActivateSwitch(b);
		}
    }
}
