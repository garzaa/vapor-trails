using UnityEngine;

public class IntInState : StateMachineBehaviour {

    public int stateNum;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        animator.SetInteger("SubState", stateNum);
    }

}