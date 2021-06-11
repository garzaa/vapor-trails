using UnityEngine;

[NodeWidth(120)]
public class SignalOnStart : ActionNode {
    [Output]
    public Signal output;

    protected override void OnInput() {
        SetPortOutput(nameof(output), Signal.positive);
    }
}
