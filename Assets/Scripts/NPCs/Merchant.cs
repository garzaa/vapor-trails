using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class Merchant : PersistentObject {
    public NPC npc;
    public InventoryList baseInventory;
    List<GameFlag> gameFlagsHit;

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
        
    }
}