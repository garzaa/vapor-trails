using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemPane : MonoBehaviour, ISelectHandler {
    InventoryUI inventoryUI;
    public InventoryItem inventoryItem;

    public Image itemImage;
    public Text itemCount;

    void Start() {
        inventoryUI = GetComponentInParent<InventoryUI>();
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (inventoryUI != null) {
            inventoryUI.ReactToItemHover(this);
        }
    }

    void OnClick() {
        GlobalController.inventory.ReactToItemSelect(this.inventoryItem);
    }

    public void PopulateSelfInfo(InventoryItem item) {
        itemImage.sprite = item.itemIcon;
        this.inventoryItem = item;
        itemCount.text = (item.count > 1 ? item.count.ToString() : "");
    }

}