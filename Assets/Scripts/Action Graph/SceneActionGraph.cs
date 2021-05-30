using UnityEngine;
using XNode;
using System.Linq;
using System.Collections.Generic;

public class SceneActionGraph : SceneGraph<ActionGraph>, IStateUpdateListener {

    bool started = false;
    bool hasStateListeners = false;

    void Start() {
        started = true;
        Initialize();
    }

    public void OnStateUpdate() {
        if (!hasStateListeners) {
            return;
        }
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
            if (node is StateChangeNode) {
                hasStateListeners = true;
            }

            // maybe put this in the node
            if (node is InteractTriggerNode) {
                (node as InteractTriggerNode).Link();
            }
        }
    }

    List<ActionNode> GetRootNodes() {
        ActionGraph g = graph;

        // this can happen due to start weirdness
        if (graph == null) {
            graph = GetComponent<ActionGraph>();
        }
        return graph.nodes
            .ConvertAll<ActionNode>(x => (ActionNode) x)
            .Where(x => x is SignalOnStart)
            .ToList();
    }
}
