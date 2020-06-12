using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnlocks : MonoBehaviour {
	public int baseDamage = 1;

	public List<Ability> unlockedAbilities = new List<Ability>();

	public bool HasAbility(Ability a) {
		return unlockedAbilities.Contains(a);
	}

	public SerializableUnlocks MakeSerializableUnlocks() {
		return new SerializableUnlocks(this);
	}

	public void LoadFromSerializableUnlocks(SerializableUnlocks p) {
		this.unlockedAbilities = p.unlockedAbilities;
		baseDamage = p.baseDamage;
	}

}

[System.Serializable]
public class SerializableUnlocks {
	public int baseDamage = 1;
	public List<Ability> unlockedAbilities;

	public SerializableUnlocks(PlayerUnlocks p) {
		this.unlockedAbilities = p.unlockedAbilities;
		baseDamage = p.baseDamage;
	}
}