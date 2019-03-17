using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AbilityItem : InventoryItem {

	public Ability ability;
	public Sprite abilitySprite;
	[TextArea]
	public string instructions;

	public override void OnPickup() {
		GlobalController.abilityUIAnimator.GetComponent<AbilityGetUI>().GetItem(this);
		GlobalController.UnlockAbility(this.ability);
	}

	new public AbilityItem Clone() {
		return (AbilityItem) this.MemberwiseClone();
	}		
}
