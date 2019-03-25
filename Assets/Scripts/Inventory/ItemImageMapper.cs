using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class ItemImageMapper : MonoBehaviour {
    static Dictionary<string, ItemImageMap> mappings;

    void Start() {
        mappings = new Dictionary<string, ItemImageMap>();

        List<InventoryItem> allItems = Resources.LoadAll(
            "InventoryItems",
            typeof(ItemWrapper)
            ).Select(
                x => ((ItemWrapper) x).GetItem()
        ).ToList();
        
        foreach (InventoryItem item in allItems) {
            if (mappings.ContainsKey(item.itemName)) {
                continue;
            }
            mappings.Add(
                item.itemName,
                new ItemImageMap(
                    item.itemIcon,
                    item.detailedIcon
                )
            );
        }
    }

    public static ItemImageMap GetMapping(string itemName) {
        return mappings[itemName];
    }
}