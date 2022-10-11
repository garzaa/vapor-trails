using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

[System.Serializable]
public class Save {
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

    public void Clear() {
        sceneName = "";
        playerPosition = Vector2.zero;
        version = Application.version;
        persistentObjects.Clear();
        gameFlags.Clear();
        gameStates.Clear();
    }

    void ResetPersistentObject() {
        foreach (PersistentObject p in Resources.FindObjectsOfTypeAll<PersistentObject>()) {
            p.Reload();
        }
    }

    Dictionary<string, object> GetSubDict(Dictionary<string, object> current, string s) {
        if (!current.ContainsKey(s)) {
            current[s] = new Dictionary<string, object>();
        } else if (current[s].GetType().Equals(typeof(JObject))){
            // if this is a JObject, convert it to an object and then a new dictionary<string, object>
            // this will come from disk saves being loaded as loosely-typed dictionaries
            Dictionary<string, object> d =  (current[s] as JObject).ToObject<Dictionary<string, object>>();
            current[s] = d;
        }
        return current[s] as Dictionary<string, object>;
    }

    Dictionary<string, object> GetDictForPath(string[] path) {
        // turn a/b/c into persistentObjects[a][b][c]
        string root = path[0];
        if (!persistentObjects.ContainsKey(root)) {
            persistentObjects[root] = new Dictionary<string, object>();
        }
        Dictionary<string, object> current = persistentObjects[root];
        for (int i=1; i<path.Length; i++) {
            current = GetSubDict(current, path[i]);
        }
        return current;
    }

public void SetPersistentObject(PersistentObject o) {
        GetDictForPath(o.GetPath())[o.GetName()] = o.GetAllProperties();
    }

    public Dictionary<string, object> GetPersistentObject(PersistentObject o) {
        object d = null;
        GetDictForPath(o.GetPath()).TryGetValue(o.GetName(), out d);
        if (d != null) {
            if (d.GetType().Equals(typeof(JObject))) {
                return (d as JObject).ToObject<Dictionary<string, object>>();
            }
        }
        return d as Dictionary<string, object>;
    }

    public void BeforeSerialize() {
        version = Application.version;
    }

    public void AfterDeserialize() {
        options.Load();
    }
}
