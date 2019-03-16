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
    public int cost;

    [TextArea]
    public string itemDescription = "";

    public virtual void OnPickup() {
    }

    public bool IsAbility() {
        return GetType() == typeof(AbilityItem);
    }
}