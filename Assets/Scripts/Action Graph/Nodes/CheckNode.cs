using UnityEngine;

public abstract class CheckNode : ActionNode {
    [Output]
    public Signal output;

    override protected void OnInput() {
        // only fire the check if there's a positive input
        if (input.value) {
            SetPortOutput(nameof(output), new Signal(Check()));
        }
    }

    protected abstract bool Check();
}
