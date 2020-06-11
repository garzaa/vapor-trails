using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class Merchant : PersistentObject {
    public List<Item> startingInventory;
    public InventoryList baseInventory;
    List<GameFlag> gameFlagsHit = new List<GameFlag>();

    public string merchantName;
    public Sprite merchantPortrit;
    [TextArea]
    public string greetingDialogue;
    [TextArea]
    public string notEnoughMoneyDialogue;
    [TextArea]
    public string thanksDialogue;

    override public void ConstructFromSerialized(SerializedPersistentObject s) {
        this.baseInventory = new InventoryList();
        if (s == null) {
            this.baseInventory.AddAll(startingInventory);
            return;
        }

        this.persistentProperties = s.persistentProperties;

        this.gameFlagsHit = ((List<int>) s.persistentProperties["GameFlags"]).Select(
            x => (GameFlag) x
        ).ToList();

        this.baseInventory.LoadFromSerializableInventoryList(
            (SerializableInventoryList) s.persistentProperties["Inventory"]
        );
    }

    public void AddGameFlagInventory(GameFlagInventory i) {
        if (gameFlagsHit.Contains(i.flag)) {
            return;
        }
        baseInventory.AddAll(i.items);
        gameFlagsHit.Add(i.flag);
    }

    override protected void UpdateObjectState() {
        this.persistentProperties = new Hashtable();
        this.persistentProperties.Add("Inventory", baseInventory.MakeSerializableInventory());
        this.persistentProperties.Add(
            "GameFlags", 
            this.gameFlagsHit.Select(f => (int) f).ToList()
        );
        SaveObjectState();
    }

    public void ReactToBuy() {
        this.UpdateObjectState();
    }
}