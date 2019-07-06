using UnityEngine;

public class RandomChoiceInState : StateMachineBehaviour {

    public float range;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        animator.SetFloat("RandomChoice", Random.Range(0f, range));
    }
}