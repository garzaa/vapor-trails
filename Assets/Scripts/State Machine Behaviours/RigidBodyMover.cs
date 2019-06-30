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
        float baseX = x;
        if (pickMax) baseX = Mathf.Max(Mathf.Abs(x), Mathf.Abs(rb2d.velocity.x)) * Mathf.Sign(x);
        if (entityForward) baseX *= e.ForwardScalar();

        rb2d.velocity = new Vector2(
            forceX ? baseX : rb2d.velocity.x,
            forceY ? y : rb2d.velocity.y
        );
    }
}