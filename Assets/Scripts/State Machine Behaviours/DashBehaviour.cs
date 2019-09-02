using UnityEngine;

public class DashBehaviour : StateMachineBehaviour {
    PlayerController pc;
    public bool backwards = false;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        pc = animator.GetComponent<PlayerController>();
        pc.StartDashAnimation(backwards);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        pc.StopDashAnimation();
    }
}