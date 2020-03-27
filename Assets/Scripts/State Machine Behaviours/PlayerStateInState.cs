using UnityEngine;

public class PlayerStateInState : StateMachineBehaviour {

    PlayerController pc;
    public PlayerStates state;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (pc == null) pc = animator.GetComponent<PlayerController>();
        pc.currentState = this.state;
    }
}