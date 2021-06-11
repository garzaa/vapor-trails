using UnityEngine;

public class GameStateCheck : CheckNode {
    public GameState state;

    override protected bool Check() {
        return GlobalController.HasState(state);
    }
}   
