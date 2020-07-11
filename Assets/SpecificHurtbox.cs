using UnityEngine;
using System.Collections.Generic;

public class SpecificHurtbox : Hurtbox {
    public string attackName;
    public List<string> attackNames;

	override public bool OnHit(Attack a) {
        if (!string.IsNullOrEmpty(attackName)) {
            if (!string.Equals(attackName, a.attackName)) {
                return false;
            }
        }

        if (!attackNames.Contains(a.attackName)) {
            return false;
        }

		return base.OnHit(a);
	}
}
