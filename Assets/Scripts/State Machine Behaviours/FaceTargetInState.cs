using UnityEngine;

public class FaceTargetInState : StateMachineBehaviour {
   FaceTarget ft;
   public bool setEnabled = true;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        ft = animator.GetComponent<FaceTarget>();
        ft.enabled = setEnabled;
    }
}
