public class SceneFlagWanter : Activatable {
    public SceneFlag sceneFlag;
    public Activatable yesActivatable;
    public Activatable noActivatable;
    public bool resetsFlag;

    override public void Activate() {
        if (!sceneFlag.gotten) {
            if (noActivatable != null) {
                noActivatable.Activate();
                if (yesActivatable != null) {
                    yesActivatable.ActivateSwitch(false);
                }
                return;
            }
        }
        yesActivatable.Activate();
        if (resetsFlag) sceneFlag.gotten = false;
    }

    override public void ActivateSwitch(bool b) {
        Activate();
    }
}