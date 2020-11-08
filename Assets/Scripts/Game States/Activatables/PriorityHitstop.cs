public class PriorityHitstop : Activatable {
    public float duration;

    public override void ActivateSwitch(bool b) {
        if (b) Hitstop.Run(duration, priority:true);
    }
}