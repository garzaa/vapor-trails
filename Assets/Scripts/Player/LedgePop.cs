using UnityEngine;

public class LedgePop : MonoBehaviour {
    const float popDistance = 0.10f;
    const float castDistance = 0.05f;
    const float speedMultiplier = 0.5f;
    const float yspeedCutoff = 3.5f;

    Collider2D col;
    Rigidbody2D rb;
    int layerMask;
    Vector2 boxPos;
    PlayerController player;

    GroundCheck groundCheck;

    void Start() {
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        layerMask = 1 << LayerMask.NameToLayer(Layers.Ground);

        player = GetComponent<PlayerController>();
        groundCheck = GetComponent<GroundCheck>();
    }

    void Update() {
        if (Mathf.Abs(rb.velocity.x) < 0.2f) return;
        if (groundCheck != null && groundCheck.groundData.grounded) return;
        boxPos = (Vector2) col.transform.position + col.offset;
        CheckPopX();
    }

    void CheckPopX() {
        float speed = rb.velocity.x;
        Vector2 forward = Vector2.right * Mathf.Sign(speed);
        float distance = (col.bounds.size.x/2f)+(castDistance * speed * speedMultiplier);

        CheckPop(Vector2.up, forward, distance);
    }

    void CheckPop(Vector2 popDirection, Vector2 forward, float distance) {
        // first: project the collider forward to mimic movement
        RaycastHit2D hit = Physics2D.BoxCast(
            origin: boxPos,
            // don't pick up what the box is currently touching
            size: (Vector2) col.bounds.size - new Vector2(0.01f, 0.01f),
            angle: 0,
            direction: forward,
            distance: distance,
            layerMask: layerMask 
        );

        // if no upcoming hit, who cares
        if (hit.transform == null) return;
        
        // if upcoming hit, then cast in the same direction with lower tolerance applied
        Vector2 pop = popDirection * popDistance;

        hit = Physics2D.BoxCast(
            origin: boxPos + pop*2,
            size: (Vector2) col.bounds.size - pop,
            angle: 0,
            direction: forward,
            distance: distance,
            layerMask: layerMask
        );


        // if no hit with inner tolerance, then pop the rb2d in that direction
        if (hit.transform == null) {
            rb.MovePosition(rb.position+pop);
            if (rb.velocity.y < yspeedCutoff) rb.velocity = new Vector2(rb.velocity.x, 0f);

            if (player != null) {
                player.OnLedgePop();
            }
        }
    }
}
