using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerUnlocks {
	public int baseDamage = 1;

	public List<Ability> unlockedAbilities = new List<Ability>();

	public bool HasAbility(Ability a) {
		return unlockedAbilities.Contains(a);
	}

	public void Clear() {
		unlockedAbilities.Clear();
		baseDamage = 1;
	}

}
