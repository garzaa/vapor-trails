using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedLimiter : MonoBehaviour {
    public float maxSpeedX;
    public float maxSpeedY;

    protected Rigidbody2D rb2d;

    virtual protected void Start() {
        rb2d = GetComponent<Rigidbody2D>();
    }

    void LateUpdate() {
        SlowRigidBody();
    }

    virtual protected void SlowRigidBody() {
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

    public bool IsMovingFast() {
        return (Mathf.Abs(rb2d.velocity.x) > maxSpeedX || Mathf.Abs(rb2d.velocity.y) > maxSpeedY/6f);
    }
}
