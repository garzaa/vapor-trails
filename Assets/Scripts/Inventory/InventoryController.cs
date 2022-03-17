using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour {
    public InventoryUI inventoryUI;
    public MerchantUI merchantUI;
    bool inInventory = false;

    public Item moneyItem;
    public Merchant currentMerchant;
    public Text moneyUI;
    public AudioSource itemBuy;
    bool initialized = false;
    private InventoryList _items = null;
    public InventoryList items {
        get {
            if (_items == null) {
                _items = GetComponent<InventoryList>();
            }
            return _items;
        }
    }

    void Awake() {
        UpdateMoneyUI();
    }

    public void ReactToItemSelect(StoredItem item) {
        if (this.currentMerchant == null)  {
            return;
        }
        TryToBuy(item);
    }

    public void AddItem(StoredItem s, bool quiet) {
		items.AddItem(s);
        if (inInventory) inventoryUI.PopulateItems(this.items);
		s.item.OnPickup(quiet);
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

    void TryToBuy(StoredItem s) {
        InventoryList merchantInventory = currentMerchant.baseInventory;
        Item item = s.item;
        bool hasMoney = item.cost <= GlobalController.inventory.CheckMoney();
        if (hasMoney) {
            // copy
            StoredItem toAdd = new StoredItem(merchantInventory.GetItem(item).item);
            TakeMoney(item.cost);
            if (merchantInventory.GetItem(item).item.stackable) {
                if (merchantInventory.GetItem(s).count > 1) {
                    merchantInventory.GetItem(s).count -= 1;
                } else {
                    merchantInventory.RemoveItem(s);
                }
            } else {
                merchantInventory.RemoveItem(s);
            }
            GlobalController.AddItem(toAdd, false);
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
