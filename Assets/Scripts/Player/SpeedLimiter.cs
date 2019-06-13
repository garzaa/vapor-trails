using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedLimiter : MonoBehaviour {
    Rigidbody2D rb2d;
    public float maxSpeedX;
    public float maxSpeedY;

    public bool damping = false;
    [Range(0, 1)]
    public float dampRate;

    const float DAMP_INTERVAL = 0.1f;
    bool waitingToDamp = false;

    void Start() {
        rb2d = GetComponent<Rigidbody2D>();
    }

    void Update() {
        if (!damping) {
            ClampSpeed();
        } else if (!waitingToDamp) {
            waitingToDamp = true;
            StartCoroutine("DampSpeed");
        }
    }

    IEnumerator DampSpeed() {
        yield return new WaitForSeconds(dampRate);
        Vector2 newVec = rb2d.velocity;
        if (Mathf.Abs(rb2d.velocity.x) > maxSpeedX) {
            newVec.x = rb2d.velocity.x - Mathf.Sign(rb2d.velocity.x)*dampRate;
        }
        if (Mathf.Abs(rb2d.velocity.y) > maxSpeedY) {
            newVec.y = rb2d.velocity.y - Mathf.Sign(rb2d.velocity.y)*dampRate;
        }

        rb2d.velocity = newVec;
        waitingToDamp = false;
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
