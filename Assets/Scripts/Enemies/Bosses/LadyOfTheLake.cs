using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadyOfTheLake : Enemy {

	public List<string> animationTriggers;
	int lastAction = 0;

	public void ChooseNextMove() {
		int currentAction = lastAction;
		while (lastAction == currentAction) {
			currentAction = Mathf.FloorToInt(Random.Range(0, animationTriggers.Count-1));
		}
		anim.SetTrigger(animationTriggers[currentAction]);
	}
}
