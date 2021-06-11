using UnityEngine;
using XNode;

public class InteractTriggerNode : ActionNode {
    public GameObject interactable;

    [Output]
    public Signal output;

    protected override void OnInput() {
        SetPortOutput(nameof(output), input);
    }

    // add a listener to the target gameobject
    // maybe abstract out a node that has to setup something with a target gameobject?
    // in the future anyway
    public void Link() {
        ActivateOnInteract a = interactable.GetComponent<ActivateOnInteract>();
        NodeInputOnActivate i = interactable.AddComponent<NodeInputOnActivate>();
        a.activatable = i;
        i.targetNode = this;
    }
}
