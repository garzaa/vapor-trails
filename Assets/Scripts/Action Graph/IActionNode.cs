using XNode;
using System.Collections.Generic;
using System.Linq;

[NodeWidth(270)]
public abstract class IActionNode : Node { 
    [Input(backingValue=ShowBackingValue.Never)]
    public bool input;

    public abstract void OnInput(bool signal);

    public bool IsRoot() {
        return !GetPort(nameof(input)).IsConnected;
    }

    public IEnumerable<IActionNode> GetActionNodes(string portName) {
        return GetPort(portName).GetConnections() as IEnumerable<IActionNode>;
    }
}

