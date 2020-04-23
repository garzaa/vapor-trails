using UnityEngine;

public class EnableOnItem : MonoBehaviour {
    [SerializeField] ItemWrapper wantedItem;
    public bool immediate = true;

    public bool setDisabled = false;

    void Start() {
        CheckState();
    }

    public void CheckState() {
        InventoryItem i = GlobalController.inventory.items.GetItem(wantedItem.item.itemName);
        bool hasItem = (i != null && i.count >= wantedItem.item.count);
        if (setDisabled) gameObject.SetActive(!hasItem);
        else gameObject.SetActive(hasItem);
    }
}