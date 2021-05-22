using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Scene Container", menuName = "Runtime/Save Container")]
public class SaveContainer : ScriptableObject
{
    [SerializeField] Save save;

    // this exists so multiple save containers can reference the same save
    // so dev saves left over in levels can use the runtime save in the final build
    [SerializeField] RuntimeSaveWrapper runtime;

    public void WipeSave() {
        runtime.inventory.Clear();
        SaveContainer newSave = Resources.Load("ScriptableObjects/Runtime/Save Containers/New") as SaveContainer;
        this.save = newSave.save;
        this.startingGameStates = newSave.startingGameStates;
        this.startingItems = newSave.startingItems;
    }

    [SerializeField] List<Item> startingItems;
    [SerializeField] List<GameState> startingGameStates;

    public Save GetSave() {
        return runtime.save;
    }

    public void OnSceneLoad() {
        if (!save.loadedOnce) {
            foreach (Item i in startingItems) {
                GlobalController.AddItem(new StoredItem(i), quiet:true);
            }
            GlobalController.AddStates(startingGameStates);
        }
        save.loadedOnce = true;

        save.Initialize();
    }

    public bool RuntimeLoadedOnce() {
        return runtime.loadedOnce;
    }

    // called before scene transitions
    public void SaveInventory(InventoryList inventory) {
        runtime.inventory = inventory;
    }

    public InventoryList GetInventory() {
        return runtime.inventory;
    }

    public void LoadFromSlot(int slot) {
        this.save = BinarySaver.LoadFile(slot);
    }

    public void WriteToDiskSlot(int slot) {
        BinarySaver.SaveFile(this.save, slot);
    }

    public void SyncImmediateStates(int slot) {
        // load a copy of the current save parallel to runtime, add/remove immediate states as necessary, and save
        // don't add any of the current runtime properties
        if (BinarySaver.HasFile(slot)) {
            Save diskSave = BinarySaver.LoadFile(slot);

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
            BinarySaver.SaveFile(diskSave, slot);
        }
    }
}
