using System;
using XNode;
using XNodeEditor;

[CustomNodeGraphEditor(typeof(ActionGraph))]
public class ActionGraphEditor : NodeGraphEditor {
    // only pick up action graph nodes
    public override string GetNodeMenuName(Type type) {
        if (typeof(ActionNode).IsAssignableFrom(type)) {
            return base.GetNodeMenuName(type);
        } else return null;
    }
}
