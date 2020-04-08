using UnityEngine;

public class TrackOffset : StateMachineBehaviour {
    public string anchorName = "Hips";

    GameObject parentContainer;
    GameObject anchor;

    Vector2 startPos;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        parentContainer = animator.gameObject;
        anchor = parentContainer.transform.Find(anchorName).gameObject;
        startPos = anchor.transform.position;
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        Vector2 endPos = anchor.transform.position;
        parentContainer.transform.position += (Vector3) endPos - (Vector3) startPos;
    }
}