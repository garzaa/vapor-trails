using UnityEngine;
using System.Collections.Generic;

public class ItemDB {
    readonly static string itemPath = "ScriptableObjects/InventoryItems/";
    static Dictionary<string, Item> itemCache = new Dictionary<string, Item>();

    public static Item GetItem(string itemName) {
        if (itemCache.ContainsKey(itemName)) {
            return itemCache[itemName];
        }

        Item item = (Item) Resources.Load(itemPath+itemName);
        itemCache.Add(itemName, item);
        return item.Instance();
    }
}