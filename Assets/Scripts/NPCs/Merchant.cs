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


    override public void Start() {
        base.Start();
        if (generateMapIcon) {
			Instantiate(Resources.Load("ShopIcon"), transform.position, Quaternion.identity, this.transform);
		}
    }

    override public void ConstructFromSerialized(SerializedPersistentObject s) {
        this.baseInventory = GetComponent<InventoryList>();
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

    public string GetThanksDialogue(Item item) {
        return thanksDialogue;
    }
}