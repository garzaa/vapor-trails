using UnityEngine;

public class FaceTowardsMovement : MonoBehaviour {
    public Entity entity;
    Rigidbody2D rb2d;

    void Start() {
        entity = entity ?? GetComponent<Entity>();
        rb2d = entity.GetComponent<Rigidbody2D>();
    }

    void LateUpdate() {
        if (entity.facingRight && rb2d.velocity.x < 0) {
            entity.Flip();
        } else if (!entity.facingRight && rb2d.velocity.x > 0) {
            entity.Flip();
        }
    }
}