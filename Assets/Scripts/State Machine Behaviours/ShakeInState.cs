using UnityEngine;

public class ShakeInState : StateMachineBehaviour {
    public float intensity;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        CameraShaker.Shake(intensity, 0);
        CameraShaker.StartShaking(  );
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        CameraShaker.StopShaking();
    }
}