using UnityEngine;

public abstract class CheckNode : ActionNode {
    [Output(backingValue=ShowBackingValue.Never)]
    public Signal yes;

    [Output(backingValue=ShowBackingValue.Never)]
    public Signal no;

    override protected void OnInput() {
        Signal output = new Signal(Check());

        foreach (ActionNode node in GetActionNodes(nameof(yes))) {
            node.SetInput(output);
        }
        
        foreach (ActionNode node in GetActionNodes(nameof(no))) {
            node.SetInput(output.Flip());
        }
    }

    protected abstract bool Check();
}
