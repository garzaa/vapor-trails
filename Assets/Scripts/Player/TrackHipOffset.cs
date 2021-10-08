using UnityEngine;

public class TrackHipOffset : StateMachineBehaviour {
    public string anchorName = "PlayerRig/Hips";

    Rigidbody2D parentContainer;
    Transform anchor;
    Vector2 startPos;
    Vector2 endPos;

    Vector2 currentPos;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (anchor == null) {
            parentContainer = animator.GetComponent<Rigidbody2D>();
            anchor = parentContainer.transform.Find(anchorName);
        }

        startPos = anchor.localPosition;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        endPos = anchor.localPosition;
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        Vector2 diff = Vector2.right * (endPos - startPos) * parentContainer.transform.localScale;
        parentContainer.MovePosition(parentContainer.position + diff);
    }
}
