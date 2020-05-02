using UnityEngine;

public class ForceTowardsPlayer : MonoBehaviour {
    public float force;
    public float maxSpeed;

    public Rigidbody2D optionalSelfTarget;
    GameObject pc;
    
    void Start() {
        if (optionalSelfTarget == null) optionalSelfTarget = GetComponent<Rigidbody2D>();
        pc = GlobalController.pc.gameObject;
    }

    void FixedUpdate() {
        optionalSelfTarget.AddForce((pc.transform.position - transform.position).normalized * force);
        if (maxSpeed > 0 && optionalSelfTarget.velocity.magnitude > maxSpeed) {
            optionalSelfTarget.velocity = optionalSelfTarget.velocity.normalized * maxSpeed;
        }
    }
}