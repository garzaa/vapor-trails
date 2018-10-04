using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivatedAnimation : Activatable {

    public Animator anim;
    public string animationName;
    public bool isTrigger;
    public bool toggleBool;

    void Start() {
        if (isTrigger && toggleBool) {
            Debug.LogWarning("brainlet alert");
        }
    }

    public override void ActivateSwitch(bool b) {
        if (b && isTrigger) {
            anim.SetTrigger(animationName);
        } else {
            if (!toggleBool) {
                anim.SetBool(animationName, b);
            } else {
                anim.SetBool(animationName, !anim.GetBool(animationName));
            }
        }
    }
	
}
