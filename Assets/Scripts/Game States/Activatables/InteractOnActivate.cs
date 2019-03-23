using UnityEngine;

public class InteractOnActivate : Activatable {

    public Interactable interactable;

    override public void Activate() {
        this.interactable.Interact(GlobalController.pc.gameObject);
    }

    override public void ActivateSwitch(bool b) {
        if (b) Activate();
    }
}