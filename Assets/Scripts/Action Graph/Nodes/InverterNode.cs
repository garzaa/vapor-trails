public class InverterNode : ActionNode {
    public Signal output;

    override protected void OnInput() {
        SetPortOutput(nameof(output), input.inverse);
    }
}
