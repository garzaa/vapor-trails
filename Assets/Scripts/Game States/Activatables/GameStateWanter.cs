using System.Collections.Generic;
using UnityEngine;

public class GameStateWanter : Activatable {
    public List<GameState> gameStates;
    public Activatable yesActivatable;
    public Activatable noActivatable;

    override public void Activate() {
        foreach (GameState s in gameStates) {
            if (!SaveManager.HasState(s)) {
                if (noActivatable != null) {
                    noActivatable.Activate();
                }
                return;
            }
            yesActivatable.Activate();
        }
    }

    override public void ActivateSwitch(bool b) {
        Activate();
    }
}
