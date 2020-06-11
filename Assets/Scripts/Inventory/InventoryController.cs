using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour {
    public InventoryList items;   
    public InventoryUI inventoryUI;
    bool inInventory = false;

    public Item moneyItem;
    public Merchant currentMerchant;
    public Text moneyUI;
    public AudioSource itemBuy;


    void Start() {
        items = items ?? new InventoryList();
        UpdateMoneyUI();
    }

    public void ReactToItemSelect(Item item) {
        if (this.currentMerchant == null)  {
            return;
        }
        TryToBuy(item);
    }

    public void AddItem(Item item) {
        SoundManager.ItemGetSound();
		items.AddItem(item);
        if (inInventory) inventoryUI.PopulateItems(this.items);
		item.OnPickup();
        UpdateMoneyUI();
    }

    public void ShowInventory() {
        if (currentMerchant != null) {
            inventoryUI.PopulateItems(currentMerchant.baseInventory);
            inventoryUI.PropagateMerchantInfo(currentMerchant);
            inventoryUI.animator.SetBool("Merchant", true);
        } else {
            inventoryUI.animator.SetBool("Merchant", false);
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
        if (items.GetItem(moneyItem) != null) {
            return items.GetItem(moneyItem).count;
        }
        return 0;
    }

    public void UpdateMoneyUI() {
        moneyUI.text = "$ " + CheckMoney().ToString();
    }

    public void TakeMoney(int amount) {
        items.GetItem(moneyItem).count -= amount;
    }

    void TryToBuy(Item item) {
        InventoryList merchantInventory = currentMerchant.baseInventory;
        bool hasMoney = item.cost <= GlobalController.inventory.CheckMoney();
        if (hasMoney) {
            Item toAdd = merchantInventory.GetItem(item).Instance();
            TakeMoney(item.cost);
            if (merchantInventory.GetItem(item).stackable) {
                if (merchantInventory.GetItem(item).count > 1) {
                    merchantInventory.GetItem(item).count -= 1;
                } else {
                    merchantInventory.RemoveItem(item);
                }
            } else {
                merchantInventory.RemoveItem(item);
            }
            toAdd.count = 1;
            AddItem(toAdd);
            inventoryUI.merchantLine.text = currentMerchant.thanksDialogue;
            itemBuy.PlayOneShot(itemBuy.clip);
            UpdateMoneyUI();
            currentMerchant.ReactToBuy();
        } else {
            inventoryUI.merchantLine.text = currentMerchant.notEnoughMoneyDialogue;
        }
        inventoryUI.PopulateItems(currentMerchant.baseInventory);
    }
}