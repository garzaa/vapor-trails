using UnityEngine;

public class GameStateCheck : CheckNode {
    public GameState state;

    override protected bool Check() {
        return SaveManager.HasState(state);
    }
}   
