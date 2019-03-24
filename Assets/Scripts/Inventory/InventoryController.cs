using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour {
    public InventoryList items;   
    public InventoryUI inventoryUI;
    bool inInventory = false;

    public ItemWrapper moneyItem;

    public Merchant currentMerchant;

    void Start () {
        if (this.items == null) {
            return;
        }
        inventoryUI.PopulateItems(this.items);
    }

    public virtual void ReactToItemSelect(InventoryItem item) {
        if (this.currentMerchant == null) return;
        else {
            this.currentMerchant.TryToBuy(item);
        }
    }

    public void AddItem(InventoryItem item) {
        SoundManager.ItemGetSound();
		items.AddItem(item);
        inventoryUI.PopulateItems(this.items);
		item.OnPickup();
    }

    public void ShowInventory() {
        SoundManager.InteractSound();
        if (currentMerchant != null) {
            inventoryUI.PopulateItems(currentMerchant.baseInventory);
        }
        inventoryUI.Show();
    }

    public void HideInventory() {
        SoundManager.InteractSound();
        if (currentMerchant != null) {
            currentMerchant = null;
        }
        inventoryUI.Hide();
        inventoryUI.PopulateItems(this.items);
    }

    public int CheckMoney() {
        return (items.GetItem(moneyItem.GetItem()) ?? new InventoryItem()).count;
    }
}