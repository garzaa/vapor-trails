using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class Merchant : PersistentObject {
    public NPC npc;
    public InventoryList baseInventory;
    List<GameFlag> gameFlagsHit;

    override public void ConstructFromSerialized(SerializedPersistentObject s) {
        base.ConstructFromSerialized(s);
        //this is most CERTAINLY a code smell
        this.gameFlagsHit = ((List<int>) s.persistentProperties["GameFlags"]).Select(
            x => (GameFlag) x
        ).ToList();
    }

    public void OpenInventory() {

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
}