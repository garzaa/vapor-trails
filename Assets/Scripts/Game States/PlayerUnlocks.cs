using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnlocks : MonoBehaviour {
	public int maxHP = 3;
	public int maxEnergy = 5;
	public int baseDamage = 1;

	public List<Ability> unlockedAbilities = new List<Ability> {
		Ability.Dash,
		Ability.DamageDash,
		Ability.GunEyes,
		Ability.DoubleJump,
		Ability.WallClimb,
		Ability.Meteor,
		Ability.Supercruise,
		Ability.UpSlash
	};

	public bool HasAbility(Ability a) {
		return unlockedAbilities.Contains(a);
	}

	public SerializableUnlocks MakeSerializableUnlocks() {
		return new SerializableUnlocks(this);
	}

	public void LoadFromSerializableUnlocks(SerializableUnlocks p) {
		this.unlockedAbilities = p.unlockedAbilities;
		maxHP = p.maxHP;
		maxEnergy = p.maxEnergy;
		baseDamage = p.baseDamage;
	}

}

[System.Serializable]
public class SerializableUnlocks {
	public int maxHP = 3;
	public int maxEnergy = 5;
	public int baseDamage = 1;
	public List<Ability> unlockedAbilities;

	public SerializableUnlocks(PlayerUnlocks p) {
		this.unlockedAbilities = p.unlockedAbilities;
		maxHP = p.maxHP;
		maxEnergy = p.maxEnergy;
		baseDamage = p.baseDamage;
	}
}