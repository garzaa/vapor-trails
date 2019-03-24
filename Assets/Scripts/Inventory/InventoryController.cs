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
        items = items ?? new InventoryList();
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
        } else {
            inventoryUI.PopulateItems(this.items);
        }
        inventoryUI.Show();
    }

    public void HideInventory() {
        SoundManager.InteractSound();
        currentMerchant = null;
        inventoryUI.Hide();
    }

    public int CheckMoney() {
        return (items.GetItem(moneyItem.GetItem()) ?? new InventoryItem()).count;
    }
}