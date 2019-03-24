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
    public string merchantDialogue;

    new void Start() {
        base.Start();
    }

    override public void ConstructFromSerialized(SerializedPersistentObject s) {
        if (s == null) {
            this.baseInventory = new InventoryList();
            this.baseInventory.AddAll(startingInventory.Select(
                x => x.GetItem()
            ).ToList());
            return;
        }
        this.persistentProperties = s.persistentProperties;
        //this is most CERTAINLY a code smell
        this.gameFlagsHit = ((List<int>) s.persistentProperties["GameFlags"]).Select(
            x => (GameFlag) x
        ).ToList();
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

    //TODO: implement
    public void TryToBuy(InventoryItem item) {
        // check for money
        // if so, remove from merchant inventory, add to player inventory
        // repopulate inventory UI
    }
}