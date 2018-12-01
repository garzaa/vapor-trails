using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerUnlocks : MonoBehaviour {
	//TODO: convert this to an enum

	//abilities
	public bool dash;
	public bool damageDash;
	public bool gunEye;
	public bool doubleJump;
	public bool wallClimb;
	public bool meteor;
	public bool supercruise;
	public bool riposte;
	public bool impactKick;
	/*
	public bool valkyrieHover;
	public bool upppercut;
	*/

	//passives
	public int maxHP;
	public int maxEnergy;
	public int baseAttackDamage = 1;
}
