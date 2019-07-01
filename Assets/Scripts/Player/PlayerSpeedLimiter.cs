using UnityEngine;

public class PlayerSpeedLimiter : SpeedLimiter {

    public float dragAmount = 0.01f;
    public bool clampSpeed = false;
    PlayerController pc;

    override protected void Start() {
        base.Start();
        pc = GetComponent<PlayerController>();
    }

    override protected void SlowRigidBody() {
		if (Mathf.Abs(rb2d.velocity.x) < 0.01f) {
			return;
		}
		float originalSign = Mathf.Sign(rb2d.velocity.x);
		if (IsSpeeding()) {
            float reduced = maxSpeedX;
            if (!clampSpeed) {
			    reduced = Mathf.Max(Mathf.Abs(rb2d.velocity.x)-dragAmount, maxSpeedX);
            }
            rb2d.velocity = new Vector2(
                reduced * originalSign,
                rb2d.velocity.y
		    );
		}
	}
}