using UnityEngine;
using XNode;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

public class SceneActionGraph : SceneGraph<ActionGraph>, IStateChangeListener {

    bool started = false;
    bool hasStateListeners = false;

    void Start() {
        started = true;
        Initialize();
    }

    public void Awake() {
		Debug.Log("registering state change listener for "+this.name);
        StateChangeRegistry.Add(this);
    }

    public void OnDestroy() {
        StateChangeRegistry.Remove(this);
    }

    public void React(bool fakeSceneLoad) {
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

        hasStateListeners = GetStateListenerNodes().Count > 0;

        foreach (ActionNode node in GetRootNodes()) {
            node.SetInput(Signal.positive);

            // maybe put this in the node
            if (node is InteractTriggerNode) {
                (node as InteractTriggerNode).Link();
            }
        }
    }

    List<ActionNode> GetRootNodes() {
        ActionGraph g = graph;
        if (graph == null) {
            Debug.Log(this.name);
        }
        return graph.nodes
            .ConvertAll<ActionNode>(x => (ActionNode) x)
            .Where(x => x is SignalOnStart)
            .ToList();
    }
}
