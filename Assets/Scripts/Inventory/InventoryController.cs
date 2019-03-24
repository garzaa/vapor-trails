using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour {
    public InventoryList items;   
    public InventoryUI inventoryUI;
    bool inInventory = false;

    public ItemWrapper moneyItem;

    public Merchant currentMerchant;

    public Text moneyUI;

    void Start () {
        items = items ?? new InventoryList();
        UpdateMoneyUI();
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
        if (inInventory) inventoryUI.PopulateItems(this.items);
		item.OnPickup();
        UpdateMoneyUI();
    }

    public void ShowInventory() {
        SoundManager.InteractSound();
        if (currentMerchant != null) {
            inventoryUI.PopulateItems(currentMerchant.baseInventory);
            inventoryUI.animator.SetBool("Merchant", true);
        } else {
            inventoryUI.PopulateItems(this.items);
        }
        inventoryUI.Show();
    }

    public void HideInventory() {
        SoundManager.InteractSound();
        currentMerchant = null;
        inventoryUI.animator.SetBool("Merchant", false);
        inventoryUI.Hide();
    }

    public int CheckMoney() {
        if (items.GetItem(moneyItem.GetItem()) != null) {
            return items.GetItem(moneyItem.GetItem()).count;
        }
        return 0;
    }

    void UpdateMoneyUI() {
        moneyUI.text = "$ " + CheckMoney().ToString();
    }
}