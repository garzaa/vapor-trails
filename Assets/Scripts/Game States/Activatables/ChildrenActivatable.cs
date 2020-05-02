public class ChildrenActivatable : Activatable {
    override public void ActivateSwitch(bool b) {
        foreach (Activatable a in GetComponentsInChildren<Activatable>()) {
            // avoid a stack overflow
            // getcomponentsinchildren returns components in this gameobject
            if (a.gameObject != this.gameObject) a.ActivateSwitch(b);
        }
    }
}
