using UnityEngine;

public class MoveYInState : RigidBodyAffector {
    public float speed;
    public MoveYOption pickMax = MoveYOption.NEITHER;
    public bool onEnter = false;
    public bool onUpdate = true;

    override protected void Enter() {
        if (!onEnter && !onUpdate) Debug.LogWarning("brainlet alert");
        if (onEnter) Move();
    }
    
    override protected void Update() {
        if (onUpdate) Move();
    }

    void Move() {
        float baseY = speed;
        if (pickMax.Equals(MoveYOption.DOWN)) {
            baseY = Mathf.Min(rb2d.velocity.y, speed);
        } else if (pickMax.Equals(MoveYOption.UP)) {
            baseY = Mathf.Max(rb2d.velocity.y, speed);
        }
        rb2d.velocity = new Vector2(
            rb2d.velocity.x,
            baseY
        );
    }
}
