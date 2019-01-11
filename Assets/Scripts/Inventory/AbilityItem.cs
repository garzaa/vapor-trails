using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityItem : InventoryItem {

	public Ability ability;
	public Sprite abilitySprite;
	public string instructions;

	public override void OnPickup() {
		GlobalController.abilityUIAnimator.GetComponent<AbilityGetUI>().GetItem(this);
		GlobalController.UnlockAbility(this.ability);
	}
		
}
