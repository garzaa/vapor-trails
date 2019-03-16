using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemPane : MonoBehaviour, ISelectHandler {
    InventoryUI inventoryUI;
    public InventoryItem inventoryItem {
        get { return inventoryItem; }
    }

    void Start() {
        inventoryUI = GetComponentInParent<InventoryUI>();
    }

    public void OnSelect(BaseEventData eventData)
    {
        inventoryUI.ReactToItemHover(this);
    }

    public void PopulateSelfInfo(InventoryItem item) {
        
    }

}