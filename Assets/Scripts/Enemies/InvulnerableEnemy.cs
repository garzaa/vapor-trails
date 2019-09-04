using UnityEngine;

public class InvulnerableEnemy : Enemy {
    override public void DamageFor(int dmg) {
        return;
    }
}