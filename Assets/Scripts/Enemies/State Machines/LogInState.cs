using UnityEngine;

public class LogInState : StateMachineBehaviour {
    public string toLog;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        Debug.Log(toLog);
    }
}