using UnityEngine;

public class RigidBodyMover : RigidBodyAffector {
    public float x;
    public float y;
    public bool forceX;
    public bool forceY;

    override protected void Update() {
        rb2d.velocity = new Vector2(
            forceX ? x : rb2d.velocity.x,
            forceY ? y : rb2d.velocity.y
        );
    }
}