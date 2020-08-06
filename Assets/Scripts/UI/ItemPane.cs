using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemPane : MonoBehaviour, ISelectHandler {
    InventoryUI inventoryUI;
    ItemViewer itemViewer;
    
    public StoredItem storedItem;


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
        GlobalController.inventory.ReactToItemSelect(this.storedItem);
    }

    public void PopulateSelfInfo(StoredItem s) {
        itemImage.sprite = s.item.itemIcon;
        this.storedItem = s;
        itemCount.text = (s.count > 1 ? s.count.ToString() : "");
    }

}