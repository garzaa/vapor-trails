using UnityEngine;

public class TitleActivatable : Activatable {
    public string title;
    public string subtitle;

    override public void Activate() {
        GlobalController.ShowTitleText(title, subtitle);
    }

    override public void ActivateSwitch(bool b) {
        if (b) Activate();
    }
}