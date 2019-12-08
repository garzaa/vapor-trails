using UnityEngine;

public class MoveY : MonoBehaviour {
    Rigidbody2D rb2d;
    public float speed;
    public MoveYOption pickMax = MoveYOption.NEITHER;

    void Start() {
        rb2d = GetComponent<Rigidbody2D>();
    }
    
    void Update() {
        float baseY = speed;
        if (pickMax.Equals(MoveYOption.DOWN)) {
            baseY = Mathf.Min(rb2d.velocity.y, speed);
        } else if (pickMax.Equals(MoveYOption.UP)) {
            baseY = Mathf.Max(rb2d.velocity.y, speed);
        }
        rb2d.velocity = new Vector2(
            rb2d.velocity.x,
            baseY
        );
    }
}
