public class SceneFlagGiver : Activatable {
    public SceneFlag flag;

    override public void ActivateSwitch(bool b) {
        if (b) flag.gotten = true;
    }
}