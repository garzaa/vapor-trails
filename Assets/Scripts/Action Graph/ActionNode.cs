using XNode;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[NodeWidth(270)]
public abstract class ActionNode : Node { 
    [Input(backingValue=ShowBackingValue.Never)]
    [SerializeField]
    protected Signal input;

    protected abstract void OnInput();

    public void SetInput(Signal signal) {
        input = signal;
        OnInput();
    }

    public bool IsRoot() {
        return !GetPort(nameof(input)).IsConnected;
    }

    public List<ActionNode> GetActionNodes(string portName) {
        return GetPort(portName)
                .GetConnections()
                .Select(connection => connection.node)
                .Cast<ActionNode>()
                .ToList();
    }
}

