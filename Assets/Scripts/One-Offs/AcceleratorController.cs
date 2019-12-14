using UnityEngine;

public class AcceleratorController : MonoBehaviour
{
    Animator animator;

    public Vector2 boostVector;
    public AudioClip[] boostSounds;

    void Start() {
        animator = GetComponent<Animator>();
    }

    void OnTriggerEnter2D(Collider2D other) {
        Rigidbody2D rb2d = other.GetComponent<Rigidbody2D>();
        PlayerController playerController = other.GetComponent<PlayerController>();
        if (playerController != null) {
            playerController.OnBoost(this);
            animator.SetTrigger("Boost");
            CameraShaker.MedShake();
            for (int i=0; i<boostSounds.Length; i++) {
                SoundManager.PlaySound(boostSounds[i]);
            }
        }
    }

    public Vector2 GetBoostVector() {
        return boostVector.Rotate(transform.rotation.z);
    }
}
