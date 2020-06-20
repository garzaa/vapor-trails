using UnityEngine;
using UnityEngine.UI;

public class ItemFrame : MonoBehaviour {
    [SerializeField]
    InventoryList inventory;

    [SerializeField]
    Item targetItem;

    Image itemImage;
    
    void OnEnable() {
        if (itemImage == null) itemImage = GetComponentInChildren<Image>();
        if (inventory.HasItem(targetItem)) {
            itemImage.sprite = targetItem.itemIcon;
        }
    }
}