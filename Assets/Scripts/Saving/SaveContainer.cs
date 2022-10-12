using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Save Container", menuName = "Runtime/Save Container")]
public class SaveContainer : ScriptableObject {
    Save _save = new Save();

    public Save save {
        get {
            return _save;
        }
    }

    public void SetSave(Save save) {
        _save = save;
    }

    public void LoadFromSlot(int slot) {
        SetSave(JsonSaver.LoadFile(slot));
    }

    public void WriteToDiskSlot(int slot) {
        JsonSaver.SaveFile(save, slot);
    }

    public void SyncImmediateStates(int slot) {
        // load a copy of the current save parallel to runtime, add/remove immediate states as necessary, and save
        // don't add any of the current runtime properties
        if (JsonSaver.HasFile(slot)) {
            Save diskSave = JsonSaver.LoadFile(slot);

            // prune old states
            List<string> toPrune = new List<string>();
            foreach (string diskState in diskSave.gameStates) {
                if ((Resources.Load("ScriptableObjects/Game States/"+diskState) as GameState).writeImmediately) {
                    if (!save.gameStates.Contains(diskState)) {
                        toPrune.Add(diskState);
                    }
                }
            }
            foreach (string s in toPrune) {
                diskSave.gameStates.Remove(s);
            }

            // add new states
            foreach (string stateName in save.gameStates) {
                if ((Resources.Load("ScriptableObjects/Game States/"+stateName) as GameState).writeImmediately) {
                    diskSave.gameStates.Add(stateName);
                }
            }
            // save the non-runtime save loaded from disk
            JsonSaver.SaveFile(diskSave, slot);
        }
    }
}
