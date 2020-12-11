using UnityEngine;
using System.Collections;

public class BlackFadeActivator : Activatable {

    public Activatable target;

    public override void ActivateSwitch(bool b) {
        if (b) {
            StartCoroutine(_FadeCallback());
        }
    }

    // TODO: have rena get up and walk off, then trigger this to remove her fight
    IEnumerator _FadeCallback() {
        GlobalController.ShortBlackFade();
        yield return new WaitForSecondsRealtime(0.5f);
        if (target != null) target.Activate();
    }
}
