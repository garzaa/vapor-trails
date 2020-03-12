using UnityEngine;

public class RandomVectorInState : StateMachineBehaviour {
    public Vector2 lowBound;
    public Vector2 highBound;

    Rigidbody2D rb2d;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (rb2d == null) rb2d = animator.GetComponent<Rigidbody2D>();
        rb2d.velocity = new Vector2(
            Random.Range(lowBound.x, highBound.x),
            Random.Range(lowBound.y, highBound.y)
        );
    }
}