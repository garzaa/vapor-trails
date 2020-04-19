using UnityEngine;

public class FollowEffectPointInState : StateMachineBehaviour {
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        animator.GetComponent<AnimationInterface>().FollowEffectPoint();
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        animator.GetComponent<AnimationInterface>().StopFollowingEffectPoint();
    }
}