using XNode;

[NodeWidth(270)]
public abstract class IActionNode : Node { 
    [Input(backingValue=ShowBackingValue.Never)]
    public Signal input;

    public abstract void OnInput(Signal signal);
}

