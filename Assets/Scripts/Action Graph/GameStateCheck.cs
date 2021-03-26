using UnityEngine;

public class GameStateCheck : IActionNode {
    public GameState state;

    [Output(dynamicPortList=true, backingValue=ShowBackingValue.Never)]
    public Signal yes;

    [Output(dynamicPortList=true, backingValue=ShowBackingValue.Never)]
    public Signal no;

    override protected void OnInput() {
        Signal output = new Signal(GlobalController.HasState(state));

        foreach (IActionNode node in GetActionNodes(nameof(yes))) {
            node.SetInput(output);
        }
        
        foreach (IActionNode node in GetActionNodes(nameof(no))) {
            node.SetInput(output);
        }
    }
}   
