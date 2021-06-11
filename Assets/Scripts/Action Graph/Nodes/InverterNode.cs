[NodeWidth(100)]
public class InverterNode : ActionNode {
    [Output]
    public Signal output;

    override protected void OnInput() {
        SetPortOutput(nameof(output), input.inverse);
    }
}
