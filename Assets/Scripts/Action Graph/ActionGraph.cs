using UnityEngine;
using XNode;

public class ActionGraph : NodeGraph {
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
