using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Save {
    public int slotNum = 1;

    public string version {
        get;
        private set;
    }

    public HashSet<GameFlag> gameFlags = new HashSet<GameFlag>();
    public HashSet<string> gameStates = new HashSet<string>();

    public string sceneName;
    
    public Vector2 playerPosition;
    public Dictionary<string, Dictionary<string, object>> persistentObjects = new Dictionary<string, Dictionary<string, object>>();

    public GameOptions options;

    public bool firstLoadHappened;

    const string enabled = "enabled";
    const string mapFog = "mapFog";

    public void Initialize() {
        // called once per save
        firstLoadHappened = false;
        sceneName = "";
        version = Application.version;
        persistentObjects.Clear();
        gameFlags.Clear();
        gameStates.Clear();
    }

    void AddSubDictIfNeeded(string s) {
        if (!persistentObjects.ContainsKey(s)) persistentObjects[s] = new Dictionary<string, object>();
    }

    public void SavePersistentObject(PersistentObject o) {
        if (o is PersistentEnabled) {
            AddSubDictIfNeeded(enabled);
            persistentObjects[enabled][o.GetID()] = o.GetAllProperties();
            return;
        }
        else if (o is MapFog) {
            AddSubDictIfNeeded(mapFog);
            persistentObjects[mapFog][o.GetID()] = o.GetAllProperties();
            return;
        }
        persistentObjects[o.GetID()] = o.GetAllProperties();
    }

    public Dictionary<string, object> GetPersistentObject(string id) {
        Dictionary<string, object> d = null;
        persistentObjects.TryGetValue(id, out d);
        return d;
    }

    public void BeforeSerialize() {
        options.Apply();
        version = Application.version;
    }

    public void AfterDeserialize() {
        options.Load();
    }
}
