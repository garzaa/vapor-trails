using UnityEngine;
using System.Collections;

public class AnimationDelay : MonoBehaviour {
    public float delay;

    public bool random;
    public float lowBound;
    public float highBound;
    
    Animator animator;

    void Start() {
        animator = GetComponent<Animator>();
        bool scaled = !animator.updateMode.Equals(AnimatorUpdateMode.UnscaledTime);
        if (random) {
            delay = Random.Range(lowBound, highBound);
        }
        if (delay > 0) {
            animator.speed = 0;
            if (scaled) {
                StartCoroutine(WaitScaled(delay));
            } else {
                StartCoroutine(WaitUnscaled(delay));
            }
        } else if (delay < 0) {
            animator.speed = 100;
            if (scaled) {
                StartCoroutine(WaitScaled(delay / 100f));
            } else {
                StartCoroutine(WaitUnscaled(delay / 100f));
            }
        }
    }

    IEnumerator WaitScaled(float interval) {
        yield return new WaitForSeconds(interval);
        animator.speed = 1;
    }

    IEnumerator WaitUnscaled(float interval) {
        yield return new WaitForSecondsRealtime(interval);
        animator.speed = 1;
    }
}