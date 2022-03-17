using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(InventoryList))]
public class Merchant : PersistentObject {

    public List<Item> startingInventory;
    public InventoryList baseInventory;
    List<GameFlag> gameFlagsHit = new List<GameFlag>();

    public string merchantName;
    public Sprite merchantPortrait;
    [TextArea]
    public string greetingDialogue;
    [TextArea]
    public string notEnoughMoneyDialogue;
    [TextArea]
    public string thanksDialogue;

    public bool generateMapIcon = true;

    protected override void SetDefaults() {
        if (!hasSavedData) {
            this.baseInventory.AddAll(startingInventory);
            return;
        }
    
        this.gameFlagsHit = (GetProperty<List<int>>("GameFlags")).Select(
            x => (GameFlag) x
        ).ToList();
    }

    void Start() {
        if (generateMapIcon) {
			//Instantiate(Resources.Load("ShopIcon"), transform.position, Quaternion.identity, this.transform);
		}
        baseInventory = GetComponent<InventoryList>();
    }

    public void AddGameFlagInventory(GameFlagInventory i) {
        if (gameFlagsHit.Contains(i.flag)) {
            return;
        }
        baseInventory.AddAll(i.items);
        gameFlagsHit.Add(i.flag);
    }

    public void ReactToBuy() {
        SetProperty(
            "GameFlags", 
            this.gameFlagsHit.Select(f => (int) f).ToList()
        );
    }

    public string GetThanksDialogue(Item item) {
        return thanksDialogue;
    }
}
