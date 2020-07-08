using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LedgePop : MonoBehaviour {
    const float popDistance = 0.10f;
    const float castDistance = 0.10f;

    BoxCollider2D box;
    Rigidbody2D rb;

    int layerMask;

    void Start() {
        box = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();

        layerMask = 1 << LayerMask.NameToLayer(Layers.Ground);
    }

    void Update() {
        if (rb.velocity.x == 0) return;

        Vector2 boxPos = (Vector2) box.transform.position + box.offset;

        // lower tolerance
        Vector2 popDirection = Vector2.up;

        // first: project the collider forward
        Vector2 forward = Vector2.right * Mathf.Sign(rb.velocity.x);

        RaycastHit2D hit = Physics2D.BoxCast(
            origin: boxPos,
            size: box.size - new Vector2(0.01f, 0.01f),
            angle: 0,
            direction: forward,
            distance: (box.size.x/2f)+castDistance,
            layerMask: layerMask 
        );

        // if no hit, return
        if (hit.transform == null) return;
        
        // if hit, then cast in the same direction with lower tolerance applied
        Vector2 pop = popDirection*popDistance;
        hit = Physics2D.BoxCast(
            origin: boxPos + pop*2,
            size: box.size - pop,
            angle: 0,
            direction: forward,
            distance: (box.size.x/2f)+castDistance,
            layerMask: layerMask
        );


        // if no hit with tolerance, then pop the rb2d in that direction
        if (hit.transform == null) {
            rb.MovePosition(rb.position+pop);
        }
    }
}