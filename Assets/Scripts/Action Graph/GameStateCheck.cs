using UnityEngine;

public class GameStateCheck : IActionNode {
    public GameState state;

    [Output(dynamicPortList=true, backingValue=ShowBackingValue.Never)]
    public bool yes;

    [Output(dynamicPortList=true, backingValue=ShowBackingValue.Never)]
    public bool no;

    override public void OnInput(bool signal) {
        if (!signal) {
            return;
        }

        bool output = GlobalController.HasState(state);


        foreach (IActionNode node in GetActionNodes(nameof(yes))) {
            node.OnInput(output);
        }
        
        // todo: pass in information with the port (or actually just set the value on that port there)
        // also todo: use a nullable signal value
        foreach (IActionNode node in GetActionNodes(nameof(no))) {
            node.OnInput(!output);
        }
    }
}   
