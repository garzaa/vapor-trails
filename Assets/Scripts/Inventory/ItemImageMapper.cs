using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class ItemImageMapper : MonoBehaviour {
    static Dictionary<string, ItemImageMap> mappings;

    // load all inventory items into memory at runtime
    // this works because 1. item images are small and 2. computers are fast
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