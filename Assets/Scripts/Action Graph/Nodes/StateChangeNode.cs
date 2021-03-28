using UnityEngine;

[NodeWidth(100)]
public class StateChangeNode : ActionNode {
    [Output]
    public Signal output;

    override protected void OnInput() {
        SetPortOutput(nameof(output), Signal.positive);
    }
}
