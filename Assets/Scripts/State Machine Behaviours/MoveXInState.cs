using UnityEngine;

public class MoveXInState : RigidBodyAffector {
    public float speed;
    public bool entityForward;
    public bool pickMax = false;
    public bool onEnter = false;
    public bool onUpdate = true;

    Entity entity;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        entity = animator.GetComponent<Entity>();
    }

    override protected void Enter() {
        if (!onEnter && !onUpdate) Debug.LogWarning("brainlet alert");
        if (onEnter) Move();
    }

    override protected void Update() {
        if (onUpdate) Move();
    }

    void Move() {
        float baseX = speed;
        if (pickMax) {
            baseX = Mathf.Max(Mathf.Abs(speed), Mathf.Abs(rb2d.velocity.x) * Mathf.Sign(speed));
            if (entityForward) {
                baseX *= entity.ForwardScalar();
            }
            rb2d.velocity = new Vector2(
                baseX,
                rb2d.velocity.y
            );
        }
    }
}