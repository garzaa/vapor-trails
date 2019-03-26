using UnityEngine;

public class FaceTowardsMovement : MonoBehaviour {
    public Enemy enemy;
    Rigidbody2D rb2d;

    void Start() {
        enemy = enemy ?? GetComponent<Enemy>();
        rb2d = enemy.GetComponent<Rigidbody2D>();
    }

    void LateUpdate() {
        if (enemy.facingRight && rb2d.velocity.x < 0) {
            enemy.Flip();
        } else if (!enemy.facingRight && rb2d.velocity.x > 0) {
            enemy.Flip();
        }
    }
}