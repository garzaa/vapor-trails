using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class Merchant : PersistentObject {
    public List<ItemWrapper> startingInventory;
    public InventoryList baseInventory;
    List<GameFlag> gameFlagsHit;

    public string merchantName;
    public Sprite merchantPortrit;
    [TextArea]
    public string greetingDialogue;
    [TextArea]
    public string notEnoughMoneyDialogue;
    [TextArea]
    public string thanksDialogue;

    override public void ConstructFromSerialized(SerializedPersistentObject s) {
        if (s == null) {
            this.baseInventory = new InventoryList();
            this.baseInventory.AddAll(startingInventory.Select(
                x => x.GetItem()
            ).ToList());
            return;
        }

        this.persistentProperties = s.persistentProperties;

        this.gameFlagsHit = ((List<int>) s.persistentProperties["GameFlags"]).Select(
            x => (GameFlag) x
        ).ToList();

        print("merchant "+this.name+" is trying to load saved items");
        this.baseInventory.items = ((List<InventoryItem>) s.persistentProperties["Inventory"]).Select(
            x => (InventoryItem) x
        ).ToList();
    }

    public void AddGameFlagInventory(GameFlagInventory i) {
        if (gameFlagsHit.Contains(i.flag)) {
            return;
        }
        baseInventory.AddAll(i.items);
        gameFlagsHit.Add(i.flag);
    }

    override protected void UpdateObjectState() {
        this.persistentProperties.Add("Inventory", baseInventory.MakeSerializableInventory());
        this.persistentProperties.Add(
            "GameFlags", 
            this.gameFlagsHit.Select(f => (int) f)
        );
    }

    public void ReactToBuy() {
        UpdateObjectState();
    }
}