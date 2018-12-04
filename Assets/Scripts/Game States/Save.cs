using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Save : MonoBehaviour {
    public List<GameFlag> gameFlags = new List<GameFlag>();
    public List<PlayerUnlocks> unlocks = new List<PlayerUnlocks>();
    // public Dictionary<PersistentObject, persistentstate??> persistentObjects = new Dictionary<>();
}
