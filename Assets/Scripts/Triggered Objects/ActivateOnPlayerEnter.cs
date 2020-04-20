public class ActivateOnPlayerEnter : PlayerTriggeredObject {
    public Activatable toActivate;
    public bool deactivateOnExit = false;

    override public void OnPlayerEnter() {
        toActivate.ActivateSwitch(true);
    }

    override public void OnPlayerExit() {
        if (deactivateOnExit) toActivate.ActivateSwitch(false);
    }
}