using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : Attack {

	public bool attackLandEvent;

	// Use this for initialization
	void Start () {
		this.attackedTags = new List<string>();
		attackedTags.Add(Tags.PlayerHurtbox);
		attackerParent = GetComponentInParent<Entity>();
	}

	override public void OnAttackLand(Entity victim) { 
		base.OnAttackLand(victim);
		if (attackLandEvent) {
			attackerParent.GetComponent<Animator>().SetTrigger("AttackLand");
		}
	}
}
