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

    [TextArea]
    public string itemDescription = "";

    public virtual void OnPickup() {
    }
}