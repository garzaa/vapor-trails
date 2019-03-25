using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AbilityItem : InventoryItem {

	new public Ability ability;
	public Sprite abilitySprite;
	[TextArea]
	new public string instructions;

	new public AbilityItem Clone() {
		return (AbilityItem) this.MemberwiseClone();
	}

	public AbilityItem(SerializableItem i) : base(i) {

	}
}
