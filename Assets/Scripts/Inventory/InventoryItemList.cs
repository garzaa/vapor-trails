using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InventoryItemList : MonoBehaviour {
    public List<InventoryItem> itemList;

    void Start() {
        Object[] temp = Resources.LoadAll("InventoryItems", typeof(InventoryItem));
        foreach (Object o in temp) {
            itemList.Add((InventoryItem) o);
        }
    }
}