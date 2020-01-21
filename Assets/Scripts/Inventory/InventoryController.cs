using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour {
    public InventoryList items;   
    public InventoryUI inventoryUI;
    public MerchantUI merchantUI;
    bool inInventory = false;

    public Item moneyItem;
    public Merchant currentMerchant;
    public Text moneyUI;
    public AudioSource itemBuy;

    public List<Item> startingItems;

    void Start() {
        // will be overwritten by deserialization, so it's OK
                                        // hmmgh
        if (startingItems == null || this.items.items.Count > 0) return;
        if (startingItems != null) {
            foreach (Item item in startingItems) {
                // get around me being lazy in the editor
                if (item != null) GlobalController.AddItem(item, quiet:true);
            }
        }
        UpdateMoneyUI();
    }
    

    public void ReactToItemSelect(Item item) {
        if (this.currentMerchant == null)  {
            return;
        }
        TryToBuy(item);
    }

    public void AddItem(Item item, bool quiet) {
		items.AddItem(item);
        if (inInventory) inventoryUI.PopulateItems(this.items);
		item.OnPickup(quiet);
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
            AddItem(toAdd, false);
            inventoryUI.merchantLine.text = currentMerchant.GetThanksDialogue(item);
            itemBuy.PlayOneShot(itemBuy.clip);
            UpdateMoneyUI();
            currentMerchant.ReactToBuy();
        } else {
            inventoryUI.merchantLine.text = currentMerchant.notEnoughMoneyDialogue;
        }
        inventoryUI.PopulateItems(currentMerchant.baseInventory);
    }
}