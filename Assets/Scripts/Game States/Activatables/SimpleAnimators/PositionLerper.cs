using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PositionLerper : SimpleAnimator {
    public GameObject target;
    public float duration;
    public AnimationCurve timingCurve;
    public Transform start;
    public Transform end;
    public Activatable endCallback;

    float startedTime;
    bool drawingLastFrame = false;

    protected override void Draw() {
        if (!drawingLastFrame) {
            startedTime = Time.time;
        }

        float moveFrac = (Time.time - startedTime) / duration;
        // don't move past the end point
        moveFrac = Mathf.Min(moveFrac, 1);

        target.transform.position = Vector2.Lerp(
            start.position,
            end.position,
            moveFrac
        );

        if (target.transform.position == end.position) {
            running = false;
            if (endCallback != null) {
                endCallback.Activate();
            }
        }

    }
}