using UnityEngine;

public class MoveX : MonoBehaviour {
    public float speed;
    public bool entityForward;
    public bool pickMax = false;

    Entity entity;
    Rigidbody2D rb2d;

    void Start() {
        rb2d = GetComponent<Rigidbody2D>();
        entity = GetComponent<Entity>();
    }

    void Update() {
        float baseX = speed;
        if (pickMax) {
            baseX = Mathf.Max(Mathf.Abs(speed), Mathf.Abs(rb2d.velocity.x) * Mathf.Sign(speed));
            if (entityForward) {
                baseX *= entity.ForwardScalar();
            }
            rb2d.velocity = new Vector2(
                baseX,
                rb2d.velocity.y
            );
        }
    }
}