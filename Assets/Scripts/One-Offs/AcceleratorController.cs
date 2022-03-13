using UnityEngine;
using System.Collections;

public class AcceleratorController : MonoBehaviour
{
    Animator animator;

    public Vector2 boostVector;
    public AudioClip[] boostSounds;
    public int resetDelay;    

    bool armed = true;

    void Start() {
        animator = GetComponent<Animator>();
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (!armed) return;
        Rigidbody2D rb2d = other.GetComponent<Rigidbody2D>();
        PlayerController playerController = other.GetComponent<PlayerController>();
        if (playerController != null) {
            playerController.OnBoost(this);
            animator.SetTrigger("Boost");
            CameraShaker.SmallShake();
            for (int i=0; i<boostSounds.Length; i++) {
                SoundManager.WorldSound(boostSounds[i]);
            }
            StartCoroutine(ReArm());
        }
    }

    public Vector2 GetBoostVector() {
        return boostVector.Rotate(transform.eulerAngles.z);
    }

    public IEnumerator ReArm() {
        armed = false;
        yield return new WaitForSeconds(resetDelay);
        armed = true;
    }
}
