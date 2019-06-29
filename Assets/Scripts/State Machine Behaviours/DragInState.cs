using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DragInState : StateMachineBehaviour {

    public float drag;
    float lastDrag;
    PlayerSpeedLimiter speedLimiter;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        speedLimiter = animator.GetComponent<PlayerSpeedLimiter>();
        lastDrag = speedLimiter.dragAmount;
        speedLimiter.dragAmount = drag;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        speedLimiter.dragAmount = drag;
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        speedLimiter.dragAmount = lastDrag;
    }
}