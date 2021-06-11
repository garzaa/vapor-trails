using UnityEngine;

public class NodeInputOnActivate : Activatable {
    [HideInInspector]
    public ActionNode targetNode;

    public override void ActivateSwitch(bool b) {
        targetNode.SetInput(new Signal(b));
    }
}
