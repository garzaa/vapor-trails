using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcceleratorController : MonoBehaviour
{

    Animator animator;

    public Vector2 knockback;

    void Start() {
        animator = GetComponent<Animator>();
    }

    void OnTriggerEnter2D(Collider2D other) {
        Rigidbody2D rb2d = other.GetComponent<Rigidbody2D>();
        if (rb2d != null) {
            rb2d.velocity = knockback.Rotate(transform.rotation.z);
            animator.SetTrigger("Boost");
        }
    }
}
