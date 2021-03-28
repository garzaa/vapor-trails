using UnityEngine;
using XNode;
using System.Linq;
using System.Collections.Generic;

public class SceneActionGraph : SceneGraph<ActionGraph>, IStateUpdateListener {

    bool started = false;

    void Start() {
        started = true;
        Initialize();
    }

    public void OnStateUpdate() {
        // DON"T initialize, instead find the state and item checkers and update those with a postive signal
        // the immediate ones, anyway
        foreach (StateChangeNode node in GetStateListenerNodes()) {
            node.SetInput(Signal.positive);
        }
    }

    List<StateChangeNode> GetStateListenerNodes() {
        return graph.nodes
            .OfType<StateChangeNode>()
            .ToList();
    }

    void Initialize() {
        if (!started) return;

        foreach (ActionNode node in GetRootNodes()) {
            node.SetInput(Signal.positive);
        }
    }

    List<ActionNode> GetRootNodes() {
        return graph.nodes
            .ConvertAll<ActionNode>(x => (ActionNode) x)
            .Where(x => x is SignalOnStart)
            .ToList();
    }
}
