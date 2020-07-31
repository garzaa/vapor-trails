using UnityEngine;
using System.Collections;

public class TemporaryActivatable : Activatable {
    public float duration;
    public Activatable target;
    public bool realTime;

    override public void ActivateSwitch(bool b) {
        if (b) {
            StartCoroutine(ActivateRoutine());
        }
    }

    IEnumerator ActivateRoutine() {
        target.ActivateSwitch(true);

        if (realTime) yield return new WaitForSecondsRealtime(duration);
        else yield return new WaitForSeconds(duration);

        target.ActivateSwitch(false);
    }

}