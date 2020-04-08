using UnityEngine;

public class TrackHipOffset : StateMachineBehaviour {
    string anchorName = "PlayerRig/Hips";

    Rigidbody2D parentContainer;
    GameObject anchor;

    Vector2 startPos;
    Vector2 stateStartPos;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (anchor == null) {
            parentContainer = animator.GetComponent<Rigidbody2D>();
            anchor = parentContainer.transform.Find(anchorName).gameObject;
        }
        startPos = anchor.transform.position;
        stateStartPos = startPos;
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        Vector2 endPos = anchor.transform.position;
        // when the state ends, track the base movement diff, but compensate
        Vector3 diff = new Vector3((endPos.x - stateStartPos.x), 0, 0);
        parentContainer.MovePosition(parentContainer.transform.position + diff/2);
    }
}