using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemPane : MonoBehaviour, ISelectHandler {
    InventoryUI inventoryUI;
    ItemViewer itemViewer;
    
    public Item inventoryItem;


    public Image itemImage;
    public Text itemCount;

    void Start() {
        itemViewer = GetComponentInParent<ItemViewer>();
        inventoryUI = GetComponentInParent<InventoryUI>();
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (inventoryUI != null) {
            inventoryUI.ReactToItemHover(this);
        }
        if (itemViewer != null) {
            itemViewer.ReactToItemHover(this);
        }
    }

    void OnClick() {
        GlobalController.inventory.ReactToItemSelect(this.inventoryItem);
    }

    public void PopulateSelfInfo(Item item) {
        itemImage.sprite = item.itemIcon;
        this.inventoryItem = item;
        itemCount.text = (item.count > 1 ? item.count.ToString() : "");
    }

}