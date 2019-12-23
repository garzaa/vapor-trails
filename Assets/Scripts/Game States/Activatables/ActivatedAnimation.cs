using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivatedAnimation : Activatable {

    public Animator anim;
    public string animationName;
    public bool isTrigger;
    public BoolEnum setBool = BoolEnum.NONE;
    public bool toggleBool;

    void Start() {
        if (isTrigger && toggleBool && setBool == BoolEnum.NONE) {
            Debug.LogWarning("brainlet alert");
        }
        if (anim == null) {
            anim = GetComponent<Animator>();
        }
    }

    public override void ActivateSwitch(bool b) {
        if (b && isTrigger) {
            AlerterText.Alert("Setting trigger" + animationName);
            anim.SetTrigger(animationName);
        } else if (setBool != BoolEnum.NONE) {
            anim.SetBool(animationName, setBool == BoolEnum.TRUE);
        } else {
            if (!toggleBool) {
                anim.SetBool(animationName, b);
            } else {
                anim.SetBool(animationName, !anim.GetBool(animationName));
            }
        }
    }
}

public enum BoolEnum {
    NONE,
    TRUE,
    FALSE
}
