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

    List<ActionNode> GetPortNodes(string portName) {
        return GetPort(portName)
                .GetConnections()
                .Select(connection => connection.node)
                .Cast<ActionNode>()
                .ToList();
    }

    public void SetPortOutput(string port, Signal signal) {
        foreach (ActionNode node in GetPortNodes(port)) {
            node.SetInput(signal);
        }
    }
}

