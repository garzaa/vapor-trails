using UnityEngine;

public class RandomTimeOnEntry : StateMachineBehaviour {
    bool enteredNaturally = true;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        // this forces a re-entry, need to track entry and exit
        if (enteredNaturally) {
            enteredNaturally = false;
            animator.Play(stateInfo.fullPathHash, layerIndex, Random.Range(0, 1f));
        } else {
            enteredNaturally = true;
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
    }
}
