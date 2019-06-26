using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedLimiter : MonoBehaviour {
    Rigidbody2D rb2d;
    public float maxSpeedX;
    public float maxSpeedY;

    public bool hasDrag = false;
    public Vector2 drag;
    public float speedRatio = 0.7f;

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
        float force_x = -drag.x * rb2d.velocity.x * speedRatio;
        float force_y = -drag.y * rb2d.velocity.y * speedRatio;
        rb2d.AddForce(new Vector2(force_x, force_y));
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
}
