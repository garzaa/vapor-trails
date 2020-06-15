using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.Serialization;

public class Save : MonoBehaviour, ISerializationCallbackReceiver {
    public int slotNum = 1;
    public int currentHP = 5;
    public int maxHP = 5;
    public int currentEnergy = 5;
    public int maxEnergy = 5;
    public int basePlayerDamage = 1;
    public List<GameFlag> gameFlags = new List<GameFlag>();

    [HideInInspector] public List<string> gameStates = new List<string>();
    [SerializeField] List<GameState> editorGameStates;

    public PlayerUnlocks unlocks;
    
    
    public string sceneName;
    public SerializableInventoryList playerItems;
    
    public Vector2 playerPosition;
    float playerX;
    float playerY;

    [System.NonSerialized]
    public Dictionary<string, SerializedPersistentObject> persistentObjects;
    public List<string> persistentObjectKeys = new List<string>();
    public List<SerializedPersistentObject> persistentObjectValues = new List<SerializedPersistentObject>(); 

    void Awake() {
        this.unlocks = GetComponent<PlayerUnlocks>();
        persistentObjects = new Dictionary<string, SerializedPersistentObject>();
    }

    void Start() {
        foreach (GameState s in editorGameStates) {
           GlobalController.AddState(s);
        }
    }

    public void SavePersistentObject(SerializedPersistentObject o) {
        persistentObjects[o.id] = o;
    }

    public SerializedPersistentObject GetPersistentObject(string id) {
        SerializedPersistentObject o = null;
        persistentObjects.TryGetValue(id, out o);
        return o;
    }

    public void LoadNewGamePlus(SerializableSave s, int slotNum) {
        this.unlocks.LoadFromSerializableUnlocks(s.unlocks);
        GlobalController.pc.maxHP = s.maxHP;
        GlobalController.pc.maxEnergy = s.maxEnergy;
        GlobalController.pc.baseDamage = s.baseDamage;
    }

    public SerializableSave MakeSerializableSave() {
        this.maxHP = GlobalController.pc.maxHP;
        this.maxEnergy = GlobalController.pc.maxEnergy;
        this.basePlayerDamage = GlobalController.pc.baseDamage;
        this.playerPosition = GlobalController.pc.transform.position;
        this.sceneName = SceneManager.GetActiveScene().path;
        this.playerItems = GlobalController.inventory.items.MakeSerializableInventory();
        return new SerializableSave(this);
    }

    // this could be generalized, but eh
    // also maybe need some error handling in here for previous, now-invalid save files
    public void LoadFromSerializableSave(SerializableSave s) {
        this.slotNum = s.slotNum;
        this.gameFlags = s.gameFlags;
        this.gameStates = s.gameStates;
        this.persistentObjects = new Dictionary<string, SerializedPersistentObject>();
        for (int i=0; i<s.persistentObjectKeys.Count; i++) {
            this.persistentObjects[s.persistentObjectKeys[i]] = s.persistentObjectValues[i];
        }
        this.sceneName = s.sceneName;
        this.playerPosition = new Vector2(s.xPos, s.yPos);
        this.unlocks.LoadFromSerializableUnlocks(s.unlocks);
        this.maxEnergy = s.maxEnergy;
        this.maxHP = s.maxHP;
        this.basePlayerDamage = s.baseDamage;
    }

    public void UnlockAbility(Ability a) {
        if (!unlocks.unlockedAbilities.Contains(a)) {
            unlocks.unlockedAbilities.Add(a);
        }
    }

    public void OnBeforeSerialize() {
        persistentObjectKeys.Clear();
        persistentObjectValues.Clear();
        foreach (KeyValuePair<string, SerializedPersistentObject> kv in persistentObjects) {
            persistentObjectKeys.Add(kv.Key);
            persistentObjectValues.Add(kv.Value);
        }
    }

    public void OnAfterDeserialize() {
        this.persistentObjects = new Dictionary<string, SerializedPersistentObject>();
        for (int i=0; i<persistentObjectKeys.Count; i++) {
            this.persistentObjects.Add(persistentObjectKeys[i], persistentObjectValues[i]);
        }
        GlobalController.inventory.items.LoadFromSerializableInventoryList(playerItems);
        GlobalController.LoadSceneToPosition(sceneName, playerPosition);
    }
}


[System.Serializable]
public class SerializableSave {
    public int slotNum = 1;
    public List<GameFlag> gameFlags;
    public List<string> gameStates;
    public SerializableUnlocks unlocks;
    public int currentHP = 5;
    public int maxHP = 5;
    public int currentEnergy = 5;
    public int maxEnergy = 5;
    public int baseDamage = 1;
    public List<string> persistentObjectKeys;
    public List<SerializedPersistentObject> persistentObjectValues;
    public string sceneName;
    public float xPos;
    public float yPos;
    public SerializableInventoryList playerItems;

    public SerializableSave(Save s) {
        this.slotNum = s.slotNum;
        this.gameStates = s.gameStates;
        this.gameFlags = s.gameFlags;
        this.unlocks = s.unlocks.MakeSerializableUnlocks();
        this.currentEnergy = s.currentEnergy;
        this.maxEnergy = s.maxEnergy;
        this.currentHP = s.currentHP;
        this.maxHP = s.maxHP;
        this.xPos = s.playerPosition.x;
        this.yPos = s.playerPosition.y;
        this.sceneName = s.sceneName;
        this.playerItems = s.playerItems;
        this.baseDamage = s.basePlayerDamage;
        persistentObjectKeys = new List<string>();
        persistentObjectValues = new List<SerializedPersistentObject>();

        foreach (KeyValuePair<string, SerializedPersistentObject> kv in s.persistentObjects) {
            persistentObjectKeys.Add(kv.Key);
            persistentObjectValues.Add(kv.Value);
        }
    }
}