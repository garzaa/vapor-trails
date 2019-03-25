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
    public AudioSource itemBuy;


    void Start () {
        items = items ?? new InventoryList();
        UpdateMoneyUI();
    }

    public void ReactToItemSelect(InventoryItem item) {
        if (this.currentMerchant == null)  {
            return;
        }
        print("pingy");
        TryToBuy(item);
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
        if (items.GetItem(moneyItem.GetItem()) != null) {
            return items.GetItem(moneyItem.GetItem()).count;
        }
        return 0;
    }

    void UpdateMoneyUI() {
        moneyUI.text = "$ " + CheckMoney().ToString();
    }

    public void TakeMoney(int amount) {
        items.GetItem(moneyItem.GetItem()).count -= amount;
    }

    void TryToBuy(InventoryItem item) {
        InventoryList merchantInventory = currentMerchant.baseInventory;
        bool hasMoney = item.cost <= GlobalController.inventory.CheckMoney();
        if (hasMoney) {
            InventoryItem toAdd = merchantInventory.GetItem(item).Clone();
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
            items.AddItem(toAdd);
            inventoryUI.merchantLine.text = currentMerchant.thanksDialogue;
            itemBuy.PlayOneShot(itemBuy.clip);
        } else {
            inventoryUI.merchantLine.text = currentMerchant.notEnoughMoneyDialogue;
        }
        inventoryUI.PopulateItems(currentMerchant.baseInventory);
    }
}