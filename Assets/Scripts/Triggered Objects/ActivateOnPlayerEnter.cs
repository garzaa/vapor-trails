using UnityEngine;

[ExecuteInEditMode]
public class ActivateOnPlayerEnter : PlayerTriggeredObject {
    public Activatable toActivate;
    public bool deactivateOnExit = false;

    void Awake() {
        gameObject.layer = LayerMask.NameToLayer(Layers.Triggers);
        if (GetComponent<Collider2D>()) {
            GetComponent<Collider2D>().isTrigger = true;
        }
    }

    override public void OnPlayerEnter() {
        if (!Application.isPlaying) return;
        toActivate.ActivateSwitch(true);
    }

    override public void OnPlayerExit() {
        if (!Application.isPlaying) return;
        if (deactivateOnExit) toActivate.ActivateSwitch(false);
    }
}
