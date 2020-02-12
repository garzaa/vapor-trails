using UnityEngine;

public class DashBehaviour : StateMachineBehaviour {
    PlayerController pc;
    public bool backwards = false;
    Rigidbody2D rigidbody2D;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        pc = animator.GetComponent<PlayerController>();
        pc.StartDashAnimation(backwards);
        if (rigidbody2D == null) {
            rigidbody2D = pc.GetComponent<Rigidbody2D>();
        }
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        rigidbody2D.velocity = new Vector2(
            rigidbody2D.velocity.x,
            Mathf.Max(rigidbody2D.velocity.y, 0)
        );
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        pc.StopDashAnimation();
    }
}