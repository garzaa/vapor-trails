using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this is a duplicated class but it exists in the editor
public class Save : MonoBehaviour {
    public int slotNum = 1;
    public int currentHP = 5;
    public int maxHP = 5;
    public int currentEnergy = 5;
    public int maxEnergy = 5;
    public List<GameFlag> gameFlags = new List<GameFlag>();
    public PlayerUnlocks unlocks;
    // public Dictionary<PersistentObject, persistentstate??> persistentObjects = new Dictionary<>();

    void Start() {
        this.unlocks = GetComponent<PlayerUnlocks>();
    }

    public SerializableSave MakeSerializableSave() {
        return new SerializableSave(this);
    }

    public void LoadFromSerializableSave(SerializableSave s) {
        this.slotNum = s.slotNum;
        this.gameFlags = s.gameFlags;
        this.unlocks.LoadFromSerializableUnlocks(s.unlocks);
    }
}



[System.Serializable]
public class SerializableSave {
    public int slotNum = 1;
    public List<GameFlag> gameFlags;
    public SerializableUnlocks unlocks;
    public int currentHP = 5;
    public int maxHP = 5;
    public int currentEnergy = 5;
    public int maxEnergy = 5;

    public SerializableSave(Save s) {
        this.slotNum = s.slotNum;
        this.gameFlags = s.gameFlags;
        this.unlocks = s.unlocks.MakeSerializableUnlocks();
        this.currentEnergy = s.currentEnergy;
        this.maxEnergy = s.maxEnergy;
        this.currentHP = s.currentHP;
        this.maxHP = s.maxHP;
    }
}