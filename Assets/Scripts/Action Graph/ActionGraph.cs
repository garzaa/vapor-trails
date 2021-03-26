using UnityEngine;
using XNode;

public class ActionGraph : NodeGraph {
    // TODO: make this return a list, there can be multiple roots as long as it's acyclic
    IActionNode GetRootNode() {
        foreach (Node i in nodes) {
            IActionNode a = i as IActionNode;
            if (a.IsRoot()) {
                return a;
            }
        }
        Debug.LogWarning("brainlet alert");
        return null;
    }
}
