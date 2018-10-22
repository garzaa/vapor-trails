using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadyOfTheLake : Enemy {

	public List<string> animationTriggers;
	int lastAction = 0;

	public void ChooseNextMove() {
		int currentAction = lastAction;
		while (currentAction == lastAction) {
			currentAction = Mathf.FloorToInt(Random.Range(0, animationTriggers.Count));
		}
		lastAction = currentAction;
		anim.SetTrigger(animationTriggers[currentAction]);
	}
}
