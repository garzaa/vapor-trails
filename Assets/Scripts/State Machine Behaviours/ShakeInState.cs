using UnityEngine;

public class ShakeInState : StateMachineBehaviour {
    public float intensity;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        // TODO: link a noise profile here
        CameraShaker.SmallShake();
        CameraShaker.StartShaking();
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        CameraShaker.StopShaking();
    }
}
