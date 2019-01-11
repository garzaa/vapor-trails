using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour {
    public string itemName = "New Item";
    public Sprite itemIcon = null;
    public Sprite detailedIcon = null;
    public bool isUnique = false;

    [TextArea]
    public string itemDescription = "";

    public virtual void OnPickup() {
    }
}