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

	
	protected override void OnTriggerEnter2D(Collider2D other) {
		if (attackerParent is Enemy && ((Enemy) attackerParent).hp <= 0) {
			return;
		}
		base.OnTriggerEnter2D(other);
	}

	override public void OnAttackLand(Entity victim, Hurtbox hurtbox) { 
		base.OnAttackLand(victim, hurtbox);
		if (attackLandEvent) {
			attackerParent.GetComponent<Animator>().SetTrigger("AttackLand");
		}
	}
}
