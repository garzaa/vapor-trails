using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : Attack {

	public bool attackLandEvent;

	void Start () {
		this.attackedTags = new List<string>();
		attackedTags.Add(Tags.PlayerHurtbox);
		attackerParent = GetComponentInParent<Entity>();
	}

	override public void OnAttackLand(Entity victim, Hurtbox hurtbox) { 
		base.OnAttackLand(victim, hurtbox);
		if (attackLandEvent) {
			attackerParent.GetComponent<Animator>().SetTrigger("AttackLand");
		}
	}
}
