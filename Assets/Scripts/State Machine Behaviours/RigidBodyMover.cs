using UnityEngine;

public class RigidBodyMover : RigidBodyAffector {
    public float x;
    public float y;
    public bool forceX;
    public bool forceY;
    public bool entityForward;
    public bool pickMax = false;
    Entity e;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        e = animator.GetComponent<Entity>();
    }

    override protected void Update() {
        rb2d.velocity = new Vector2(
            forceX ? (pickMax ? (Mathf.Max(GetBase(), rb2d.velocity.x))*e.ForwardScalar() : GetBase()) : GetBase(),
            forceY ? y : rb2d.velocity.y
        );
    }

    float GetBase() {
        return (entityForward ? x * e.ForwardScalar() : x);
    }
}