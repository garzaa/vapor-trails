using UnityEngine;

public class TurnToVelocity : MonoBehaviour {
    public Rigidbody2D rb2d;

    void Start() {
        if (rb2d == null) {
            rb2d = GetComponent<Rigidbody2D>();
        }
    }

    void Update() {
        this.transform.eulerAngles = new Vector3(
            0,
            0,
            Vector2.Angle(Vector2.right, rb2d.velocity)
        );
    }
}