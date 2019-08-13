using UnityEngine;

public class ChanceActivatable : Activatable {
    public Activatable target;
    public float chance;

    override public void ActivateSwitch(bool b) {
        if (b && Random.value > chance) {
            target.Activate();
        }
    }
}