using UnityEngine;
using UnityEngine.UI;

public class ItemFrame : MonoBehaviour {
    [SerializeField]
    Item targetItem;

    [SerializeField] Image itemImage;
    bool initialized;

    void Start() {
        itemImage.sprite = targetItem.itemIcon;
        initialized = true;
    }
    
    void OnEnable() {
        if (!initialized) Start();
        if (GlobalController.inventory.items.HasItem(targetItem)) {
            itemImage.enabled = true;
        } else {
            itemImage.enabled = false;
        }
    }
}