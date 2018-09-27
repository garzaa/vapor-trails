using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapEnemy : Enemy {

	public BoxCollider2D triggerZone;

    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == Tags.Player) {
            anim.SetTrigger("punch");
        }
    }

}