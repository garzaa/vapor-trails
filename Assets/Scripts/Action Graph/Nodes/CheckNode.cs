using UnityEngine;

public abstract class CheckNode : ActionNode {
    [Output]
    public Signal output;

    override protected void OnInput() {
        if (input.value) {
            SetPortOutput(nameof(output), new Signal(Check()));
        } else {
            SetPortOutput(nameof(output), Signal.negative);
        }
    }

    protected abstract bool Check();
}
