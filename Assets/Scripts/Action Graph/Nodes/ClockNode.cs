using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ClockNode : ActionNode {
    [Output]
    public Signal output;

    public SceneActionGraph parentGraph;
    public float intervalSeconds;

    private Coroutine clockRoutine = null;

    protected override void OnInput() {
        if (input.value) {
            StartClock();
        } else {
            StopClock();
        }
    }

    void StartClock() {
        if (clockRoutine == null) {
            clockRoutine = parentGraph.StartCoroutine(this.ClockCycle());
        }
    }

    void StopClock() {
        if (clockRoutine != null) {
            parentGraph.StopCoroutine(clockRoutine);
        }
    }

    IEnumerator ClockCycle() {
        SetPortOutput(nameof(output), Signal.positive);

        yield return new WaitForSeconds(intervalSeconds);
        StartClock();
    }
}
