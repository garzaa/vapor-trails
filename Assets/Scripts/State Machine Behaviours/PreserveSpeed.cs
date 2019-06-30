using UnityEngine;

public class PreserveSpeed : RigidBodyAffector {
    Vector2 originalVelocity;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        originalVelocity = rb2d.velocity;
    }

    override protected void Update() {
        rb2d.velocity = originalVelocity;
    }
}