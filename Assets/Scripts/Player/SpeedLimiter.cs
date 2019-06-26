using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedLimiter : MonoBehaviour {
    public float maxSpeedX;
    public float maxSpeedY;

    public Vector2 velocity;

    Rigidbody2D rb2d;
    PlayerController pc;

    void Start() {
        rb2d = GetComponent<Rigidbody2D>();
        pc = GetComponent<PlayerController>();
    }

    void LateUpdate() {
        if (pc == null) {
            ClampSpeed();
        } else {
            ReduceSpeed();
        }
        velocity = rb2d.velocity;
    }

    void ClampSpeed() {
        Vector2 newVec = rb2d.velocity;
        if (Mathf.Abs(rb2d.velocity.x) > maxSpeedX) {
            newVec.x = rb2d.velocity.x > 0 ? maxSpeedX : -maxSpeedX;
        }
        if (Mathf.Abs(rb2d.velocity.y) > maxSpeedY) {
            newVec.y = rb2d.velocity.y > 0 ? maxSpeedY : -maxSpeedY;
        }

        rb2d.velocity = newVec;
    }

    public bool IsSpeeding() {
        return (Mathf.Abs(rb2d.velocity.x) > maxSpeedX || Mathf.Abs(rb2d.velocity.y) > maxSpeedY);
    }

    void ReduceSpeed() {
		if (Mathf.Abs(rb2d.velocity.x) < 0.01f) {
			return;
		}
		float originalSign = Mathf.Sign(rb2d.velocity.x);
		float reduced;
		if (IsSpeeding()) {
			reduced = Mathf.Max(Mathf.Abs(rb2d.velocity.x)-.1f, maxSpeedX);
            rb2d.velocity = new Vector2(
                reduced * originalSign,
                rb2d.velocity.y
		    );
		}
	}

}
