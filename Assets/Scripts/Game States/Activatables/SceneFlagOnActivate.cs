public class SceneFlagOnActivate : Activatable {
	public SceneFlag flag;

	public override void ActivateSwitch(bool b) {
		if (b) flag.gotten = true;
	}
}
