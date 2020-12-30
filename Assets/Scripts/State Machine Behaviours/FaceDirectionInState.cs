using UnityEngine;

public class FaceDirectionInState : StateMachineBehaviour {
   FaceTarget ft;
   public bool right = true;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        ft = animator.GetComponent<FaceTarget>();
        ft.enabled = false;
        Entity e = animator.GetComponent<Entity>();
        if (e != null) {
            if ((right && !e.facingRight) || (!right && e.facingRight)) {
                e.ForceFlip();
            }
        } else {
            e.transform.localScale = new Vector3(
                right ? 1 : -1,
                e.transform.localScale.y,
                e.transform.localScale.z
            );
        }
    }
}
