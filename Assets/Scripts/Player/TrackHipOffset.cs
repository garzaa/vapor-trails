using UnityEngine;

public class TrackHipOffset : StateMachineBehaviour {
    string anchorName = "PlayerRig/Hips";

    Rigidbody2D parentContainer;
    GameObject anchor;

    // the idle pos for the hips
    Vector2 startPos = new Vector2(0, 0);
    Vector2 stateStartPos;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (anchor == null) {
            parentContainer = animator.GetComponent<Rigidbody2D>();
            anchor = parentContainer.transform.Find(anchorName).gameObject;
        }
        //startPos = anchor.transform.position;
        // player hips are basically at 0, 0 compared to the rig (+/- half a pixel or whatever)
        startPos = parentContainer.transform.position;
        stateStartPos = startPos;
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        Vector2 endPos = anchor.transform.position;
        // when the state ends, track the base movement diff, but compensate
        Vector2 diff = new Vector3((endPos.x - stateStartPos.x), 0);
        parentContainer.MovePosition(parentContainer.position + diff);
    }
}