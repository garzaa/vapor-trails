using UnityEngine;
using System.Collections;

public class TemporaryActivatable : Activatable {
    [SerializeField] float duration;
    [SerializeField] Activatable target;
    [SerializeField] bool realTime;

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