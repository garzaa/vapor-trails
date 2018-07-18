using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : Attack {

	// Use this for initialization
	void Start () {
		this.attackedTags = new List<string>();
		attackedTags.Add(Tags.PlayerHurtbox);
		attackerParent = GetComponentInParent<Entity>();
	}
}
