using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// this is a duplicated class but I want it to exist in the editor
public class Save : MonoBehaviour {
    public int slotNum = 1;
    public int currentHP = 5;
    public int maxHP = 5;
    public int currentEnergy = 5;
    public int maxEnergy = 5;
    public List<GameFlag> gameFlags = new List<GameFlag>();
    public PlayerUnlocks unlocks;
    public Dictionary<string, SerializedPersistentObject> persistentObjects;
    public string sceneName;
    public Vector2 playerPosition;

    void Awake() {
        this.unlocks = GetComponent<PlayerUnlocks>();
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

    public SerializableSave MakeSerializableSave() {
        this.playerPosition = GlobalController.pc.transform.position;
        this.sceneName = SceneManager.GetActiveScene().path;
        return new SerializableSave(this);
    }

    public void LoadFromSerializableSave(SerializableSave s) {
        this.slotNum = s.slotNum;
        this.gameFlags = s.gameFlags;
        this.persistentObjects = new Dictionary<string, SerializedPersistentObject>();
        for (int i=0; i<s.persistentObjectKeys.Count; i++) {
            this.persistentObjects[s.persistentObjectKeys[i]] = s.persistentObjectValues[i];
        }
        this.sceneName = s.sceneName;
        this.playerPosition = new Vector2(s.xPos, s.yPos);
        this.unlocks.LoadFromSerializableUnlocks(s.unlocks);

        if (!Application.isEditor) {
            GlobalController.LoadSceneToPosition(sceneName, playerPosition);
        } else {
            GlobalController.MovePlayerTo(playerPosition);
        }
    }

    public void UnlockAbility(Ability a) {
        if (!unlocks.unlockedAbilities.Contains(a)) {
            unlocks.unlockedAbilities.Add(a);
        }
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
    //ublic Dictionary<string, SerializedPersistentObject> persistentObjects;
    public List<string> persistentObjectKeys;
    public List<SerializedPersistentObject> persistentObjectValues;
    public string sceneName;
    public float xPos;
    public float yPos;

    public SerializableSave(Save s) {
        this.slotNum = s.slotNum;
        this.gameFlags = s.gameFlags;
        this.unlocks = s.unlocks.MakeSerializableUnlocks();
        this.currentEnergy = s.currentEnergy;
        this.maxEnergy = s.maxEnergy;
        this.currentHP = s.currentHP;
        this.maxHP = s.maxHP;
        this.xPos = s.playerPosition.x;
        this.yPos = s.playerPosition.y;
        this.sceneName = s.sceneName;
        persistentObjectKeys = new List<string>();
        persistentObjectValues = new List<SerializedPersistentObject>();

        foreach (KeyValuePair<string, SerializedPersistentObject> kv in s.persistentObjects) {
            persistentObjectKeys.Add(kv.Key);
            persistentObjectValues.Add(kv.Value);
        }
    }
}