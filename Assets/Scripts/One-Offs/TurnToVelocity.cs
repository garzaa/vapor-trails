using UnityEngine;

public class TurnToVelocity : MonoBehaviour {
    public Rigidbody2D rb2d;
    // this is the default if entities are normally facing up
    public float offset = 90;

    void Start() {
        if (rb2d == null) {
            rb2d = GetComponent<Rigidbody2D>();
        }
    }

    void LateUpdate() {
        this.transform.eulerAngles = new Vector3(
            0,
            0,
            Vector2.SignedAngle(
                Vector2.right,
                rb2d.velocity
            ) + offset
        );
    }

    void OnDisable() {
        transform.rotation = Quaternion.identity;
    }
}
