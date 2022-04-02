using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Save Container", menuName = "Runtime/Save Container")]
public class SaveContainer : ScriptableObject {
    #pragma warning disable 0649

    // this exists so multiple save containers can reference the same save
    // so dev saves left over in levels can use the runtime save in the final build
    [SerializeField] RuntimeSaveWrapper runtime;

    // this should always be empty for a new game
    [SerializeField] List<Item> startingItems;
    [SerializeField] List<GameState> startingGameStates;
    [Tooltip("Import all items and states from the past saves")]
    [SerializeField] List<SaveContainer> compositeSaves;

    #pragma warning restore 0649

    public Save GetSave() {
        return runtime.save;
    }

    // only called when play mode is exited
    public void CleanEditorRuntime() {
        runtime.save.Clear();
    }

    public void SetRuntimeFromSelfData() {
        runtime.save.Clear();
        List<Item> allStartingItems = GetStartingItems();
        List<GameState> allStartingStates = GetStartingStates();

        foreach (Item i in allStartingItems) {
            if (!i) continue;
            GlobalController.AddItem(new StoredItem(i), quiet:true);
        }
        SaveManager.AddStates(allStartingStates);
    }

    protected List<Item> GetStartingItems() {
        List<Item> items = new List<Item>();
        items.AddRange(startingItems);
        foreach (SaveContainer c in compositeSaves) {
            items.AddRange(c.GetStartingItems());
        }
        return items;
    }

    protected List<GameState> GetStartingStates() {
        List<GameState> states = new List<GameState>();
        states.AddRange(startingGameStates);
        foreach (SaveContainer c in compositeSaves) {
            states.AddRange(c.GetStartingStates());
        }
        return states;
    }

    public void LoadFromSlot(int slot) {
        runtime.save = JsonSaver.LoadFile(slot);
    }

    public void WriteToDiskSlot(int slot) {
        JsonSaver.SaveFile(runtime.save, slot);
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
                    if (!runtime.save.gameStates.Contains(diskState)) {
                        toPrune.Add(diskState);
                    }
                }
            }
            foreach (string s in toPrune) {
                diskSave.gameStates.Remove(s);
            }

            // add new states
            foreach (string stateName in runtime.save.gameStates) {
                if ((Resources.Load("ScriptableObjects/Game States/"+stateName) as GameState).writeImmediately) {
                    diskSave.gameStates.Add(stateName);
                }
            }
            // save the non-runtime save loaded from disk
            JsonSaver.SaveFile(diskSave, slot);
        }
    }
}
