using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedLimiter : MonoBehaviour {
    Rigidbody2D rb2d;
    public float maxSpeedX;
    public float maxSpeedY;

    public bool hasDrag = false;
    public Vector2 drag;
    public float acceleration = 10f;

    const float DAMP_INTERVAL = 0.1f;
    bool waitingToDamp = false;

    public Vector2 velocity;

    void Start() {
        rb2d = GetComponent<Rigidbody2D>();
    }

    void Update() {
        if (!hasDrag) {
            ClampSpeed();
        } else if (hasDrag) {
            StartCoroutine(ApplyDrag());
        }
    }

    IEnumerator ApplyDrag() {
        yield return new WaitForFixedUpdate();
        velocity = transform.InverseTransformDirection(rb.velocity);
        float force_x = -drag.x / velocity.x;
        float force_y = -drag.y / rb2d.velocity.y;
        rb2d.AddRelativeForce(new Vector2(force_x, force_y));
        rb2d.AddRelativeForce(new Vector2(rb2d.mass * acceleration, rb2d.mass * acceleration));
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
}
