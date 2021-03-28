public class InverterNode : ActionNode {
    public Signal output;

    override protected void OnInput() {
        foreach (ActionNode node in GetActionNodes(nameof(output))) {
            node.SetInput(this.input.Flip());
        }
    }
}
