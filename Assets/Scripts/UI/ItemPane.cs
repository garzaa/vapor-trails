using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemPane : MonoBehaviour, ISelectHandler {
    InventoryUI inventoryUI;
    ItemViewer itemViewer;
    
    public Item inventoryItem;


    public Image itemImage;
    public Text itemCount;

    Button b;

    void Start() {
        itemViewer = GetComponentInParent<ItemViewer>();
        inventoryUI = GetComponentInParent<InventoryUI>();
        b = GetComponent<Button>();
        if (b != null) b.onClick.AddListener(OnClick);
    }

    public void OnSelect(BaseEventData eventData) {
        if (inventoryUI != null) {
            inventoryUI.ReactToItemHover(this);
        }
        if (itemViewer != null) {
            itemViewer.ReactToItemHover(this);
        }
    }

    void OnClick() {
        // for merchants
        GlobalController.inventory.ReactToItemSelect(this.inventoryItem);
    }

    public void PopulateSelfInfo(Item item) {
        itemImage.sprite = item.itemIcon;
        this.inventoryItem = item;
        itemCount.text = (item.count > 1 ? item.count.ToString() : "");
    }

}