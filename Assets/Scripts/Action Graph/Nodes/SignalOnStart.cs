using UnityEngine;

public class SignalOnStart : ActionNode {
    [Output]
    public Signal output;

    protected override void OnInput() {
        SetPortOutput(nameof(output), Signal.positive);
    }
}
