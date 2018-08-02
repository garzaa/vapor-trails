using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivatedAnimation : Activatable {

    public Animator anim;
    public string animationName;
    public bool isTrigger;

    public override void ActivateSwitch(bool b) {
        if (b && isTrigger) {
            anim.SetTrigger(animationName);
        } else {
            anim.SetBool(animationName, b);
        }
    }
	
}
