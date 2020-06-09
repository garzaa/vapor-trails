using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToonMotionOverride : StateMachineBehaviour {
    ToonMotion toonMotion;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (toonMotion == null) {
            toonMotion = animator.GetComponentInChildren<ToonMotion>();
        }
        toonMotion.ForceUpdate();
    }
}
