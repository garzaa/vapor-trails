using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnlocks : MonoBehaviour {
	// TODO: implement this
	public Dictionary<PassiveStat, int> passives;

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

	public int GetPassiveStat(PassiveStat p) {
		return passives[p];
	}

	public SerializableUnlocks MakeSerializableUnlocks() {
		return new SerializableUnlocks(this);
	}

	public void LoadFromSerializableUnlocks(SerializableUnlocks s) {
		this.unlockedAbilities = s.unlockedAbilities;
		this.passives = s.passives;
	}

}

public enum Ability {
	Dash,
	DamageDash,
	GunEyes,
	DoubleJump,
	WallClimb,
	Meteor,
	Supercruise,
	UpSlash,
	Parry,
	None,
	Heal
}

public enum PassiveStat {
	MaxHP,
	MaxEnergy,
	BaseAttackDamage
}

[System.Serializable]
public class SerializableUnlocks {
	public Dictionary<PassiveStat, int> passives;
	public List<Ability> unlockedAbilities;

	public SerializableUnlocks(PlayerUnlocks p) {
		this.passives = p.passives;
		this.unlockedAbilities = p.unlockedAbilities;
	}
}