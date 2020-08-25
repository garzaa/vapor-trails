using UnityEngine;

public class TrackHipOffset : StateMachineBehaviour {
    string anchorName = "PlayerRig/Hips";

    Rigidbody2D parentContainer;
    Transform anchor;
    Vector2 startPos;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (anchor == null) {
            parentContainer = animator.GetComponent<Rigidbody2D>();
            if (anchor == null) anchor = parentContainer.transform.Find(anchorName);
        }

        startPos = anchor.localPosition;
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        Vector2 endPos = anchor.localPosition;
        Vector2 diff = Vector2.right * (endPos - startPos) * parentContainer.transform.localScale;
        parentContainer.MovePosition(parentContainer.position + diff);
    }
}