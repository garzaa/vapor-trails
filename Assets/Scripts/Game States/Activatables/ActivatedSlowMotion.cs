using UnityEngine;
using System.Collections;

public class ActivatedSlowMotion : Activatable {

    public float duration = 0.2f;

    override public void ActivateSwitch(bool b) {
        if (b) {
            if (duration > 0) StartCoroutine(SlowMotion());
            else GlobalController.EnterSlowMotion();
        } else {
            GlobalController.ExitSlowMotion();
        }
    }

    IEnumerator SlowMotion() {
        GlobalController.EnterSlowMotion();
        yield return new WaitForSecondsRealtime(duration);
        GlobalController.ExitSlowMotion();
    }
}