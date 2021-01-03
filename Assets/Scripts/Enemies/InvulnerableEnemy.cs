using UnityEngine;

public class InvulnerableEnemy : Enemy {
    public bool sendHurtAnimTrigger = true;

    override public void DamageFor(int dmg) {
        if (sendHurtAnimTrigger && anim != null) {
            anim.SetTrigger("Hurt");
        }
        return;
    }
}
