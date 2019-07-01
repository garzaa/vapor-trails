using UnityEngine;

public class DashBehaviour : StateMachineBehaviour {
    PlayerController pc;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        pc = animator.GetComponent<PlayerController>();
        pc.StartDashAnimation();
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        pc.StopDashAnimation();
    }
}