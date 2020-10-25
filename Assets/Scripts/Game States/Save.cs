using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Save {
    public int slotNum = 1;
    public int currentHP = 5;
    public int maxHP = 5;
    public int currentEnergy = 5;
    public int maxEnergy = 5;
    public int basePlayerDamage = 1;
    public List<GameFlag> gameFlags = new List<GameFlag>();

    // [HideInInspector]
    public List<string> gameStates = new List<string>();

    public PlayerUnlocks unlocks;    
    
    public string sceneName;
    public SerializableInventoryList playerItems;
    
    [System.NonSerialized]
    public Vector2 playerPosition;

    float playerX;
    float playerY;

    [System.NonSerialized]
    public Dictionary<string, SerializedPersistentObject> persistentObjects = new Dictionary<string, SerializedPersistentObject>();
    
    public List<string> persistentObjectKeys = new List<string>();
    public List<SerializedPersistentObject> persistentObjectValues = new List<SerializedPersistentObject>(); 

    public GameOptions options;

    void Awake() {
        persistentObjects = new Dictionary<string, SerializedPersistentObject>();
    }

    public void SavePersistentObject(SerializedPersistentObject o) {
        persistentObjects[o.id] = o;
    }

    public SerializedPersistentObject GetPersistentObject(string id) {
        SerializedPersistentObject o = null;
        persistentObjects.TryGetValue(id, out o);
        return o;
    }

    public void LoadNewGamePlus(Save s, int slotNum) {
        this.unlocks = s.unlocks;
        GlobalController.pc.maxHP = s.maxHP;
        GlobalController.pc.maxEnergy = s.maxEnergy;
        GlobalController.pc.baseDamage = s.basePlayerDamage;
    }

    public void UnlockAbility(Ability a) {
        if (!unlocks.unlockedAbilities.Contains(a)) {
            unlocks.unlockedAbilities.Add(a);
        }
    }

    public void BeforeSerialize() {
        options.Apply();

        playerX = playerPosition.x;
        playerY = playerPosition.y;

        persistentObjectKeys.Clear();
        persistentObjectValues.Clear();
        foreach (KeyValuePair<string, SerializedPersistentObject> kv in persistentObjects) {
            persistentObjectKeys.Add(kv.Key);
            persistentObjectValues.Add(kv.Value);
        }
    }

    public void AfterDeserialize() {
        this.persistentObjects = new Dictionary<string, SerializedPersistentObject>();
        for (int i=0; i<persistentObjectKeys.Count; i++) {
            this.persistentObjects.Add(persistentObjectKeys[i], persistentObjectValues[i]);
        }

        playerPosition = new Vector2(playerX, playerY);
        options.Load();
    }
}
