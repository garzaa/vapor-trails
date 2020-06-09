using UnityEngine;

public class SpecificHurtbox : Hurtbox {
    public string attackName;

	override public bool OnHit(Attack a) {
        if (!string.Equals(attackName, a.attackName)) {
            return false;
        }

		return base.OnHit(a);
	}
}
