using UnityEngine;

public class ForceTowardsPlayer : MonoBehaviour {
    public float force;
    public float maxSpeed;

    Rigidbody2D rb2d;
    GameObject pc;
    
    void Start() {
        rb2d = GetComponent<Rigidbody2D>();
        pc = GlobalController.pc.gameObject;
    }

    void FixedUpdate() {
        rb2d.AddForce((pc.transform.position - transform.position).normalized * force);
        if (maxSpeed > 0 && rb2d.velocity.magnitude > maxSpeed) {
            rb2d.velocity = rb2d.velocity.normalized * maxSpeed;
        }
    }
}