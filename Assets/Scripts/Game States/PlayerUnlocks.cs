using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnlocks : MonoBehaviour {
	// TODO: fix this
	Dictionary<PassiveStat, int> passives;

	List<Ability> unlockedAbilities = new List<Ability> {
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