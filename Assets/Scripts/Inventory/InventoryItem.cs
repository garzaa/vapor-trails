using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class InventoryItem : System.Object {
    public string itemName = "New Item";
    public Sprite itemIcon = null;
    public Sprite detailedIcon = null;
    public bool stackable = false;
    public int count = 1;
    public int cost = 0;

    [TextArea]
    public string itemDescription = "";

    public bool isAbilityItem;
    public Ability ability = Ability.None;
	[TextArea]
	public string instructions = "";

    public virtual void OnPickup() {
        if (!IsAbility()) return;
        GlobalController.abilityUIAnimator.GetComponent<AbilityGetUI>().GetItem(this);
		GlobalController.UnlockAbility(this.ability);
    }

    public bool IsAbility() {
        return this.isAbilityItem;
    }

    public InventoryItem Clone() {
        return (InventoryItem) this.MemberwiseClone();
    }

    public SerializableItem MakeSerialized() {
        return new SerializableItem(this);
    }

    public InventoryItem(SerializableItem i) {
        this.itemName = i.itemName;
        this.stackable = i.stackable;
        this.count = i.count;
        this.cost = i.cost;
        this.itemDescription = i.itemDescription;
        this.isAbilityItem = i.isAbilityItem;
        this.ability = i.ability;
        this.instructions = i.instructions;
        ItemImageMap m = ItemImageMapper.GetMapping(this.itemName);
        this.itemIcon = m.itemThumbnail;
        this.detailedIcon = m.itemDetail;
    }
}


[System.Serializable]
public class SerializableItem {
    public string itemName = "New Item";
    public bool stackable = false;
    public int count = 1;
    public int cost;
    public string itemDescription = "";
    public bool isAbilityItem;
    public Ability ability;
	public string instructions;

    public SerializableItem(InventoryItem i) {
        this.itemName = i.itemName;
        this.stackable = i.stackable;
        this.count = i.count;
        this.cost = i.cost;
        this.itemDescription = i.itemDescription;
        this.isAbilityItem = i.isAbilityItem;
        this.ability = i.ability;
        this.instructions = i.instructions;
    }
}