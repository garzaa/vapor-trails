using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnlocks : MonoBehaviour {
	// TODO: implement this
	public Dictionary<PassiveStat, int> passives;

	public List<Ability> unlockedAbilities = new List<Ability> {
		Ability.Dash,
		Ability.DamageDash,
		Ability.GunEye,
		Ability.DoubleJump,
		Ability.WallClimb,
		Ability.Meteor,
		Ability.Supercruise,
		Ability.Riposte,
		Ability.ImpactKick
	};

	public bool HasAbility(Ability a) {
		return unlockedAbilities.Contains(a);
	}

	public int GetPassiveStat(PassiveStat p) {
		return passives[p];
	}

	public SerializableUnlocks MakeSerializableUnlocks() {
		Debug.Log(new SerializableUnlocks(this));
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
	GunEye,
	DoubleJump,
	WallClimb,
	Meteor,
	Supercruise,
	Riposte,
	ImpactKick
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