using UnityEngine;

public abstract class CheckNode : ActionNode {
    [Output(backingValue=ShowBackingValue.Never)]
    public Signal yes;

    [Output(backingValue=ShowBackingValue.Never)]
    public Signal no;

    override protected void OnInput() {
        Signal output = new Signal(Check());

        SetPortOutput(nameof(yes), output);
        SetPortOutput(nameof(no), output.inverse);
    }

    protected abstract bool Check();
}
