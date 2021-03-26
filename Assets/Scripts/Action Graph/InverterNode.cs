public class InverterNode : IActionNode {
    public Signal output;

    override protected void OnInput() {
        foreach (IActionNode node in GetActionNodes(nameof(output))) {
            node.SetInput(this.input.Flip());
        }
    }
}
