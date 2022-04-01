using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateGiver: Activatable {
	public GameState gameState;

	public override void ActivateSwitch(bool b) {
		if (b) SaveManager.AddState(this.gameState);
	}
}
