using UnityEngine;

public class TimeFaceTarget : StateMachineBehaviour {
    public float time;
    public bool setEnabled;
    
    FaceTarget faceTarget;
    bool firedThisRun = false;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        firedThisRun = false;
        if (faceTarget == null) faceTarget = animator.GetComponent<FaceTarget>();
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (!firedThisRun && stateInfo.normalizedTime >= time) {
            faceTarget.enabled = setEnabled;
            firedThisRun = true;
        }
    }
}