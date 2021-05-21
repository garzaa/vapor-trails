using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Scene Container", menuName = "Runtime/Save Container")]
public class SaveContainer : ScriptableObject
{
    [SerializeField] Save save;


    public void WipeSave() {
        this.runtimeInventory.Clear();
        SaveContainer newSave = Resources.Load("ScriptableObjects/Runtime/Saves/New Save") as SaveContainer;
        this.save = newSave.save;
        this.startingGameStates = newSave.startingGameStates;
        this.startingItems = newSave.startingItems;
    }

    [SerializeField] List<Item> startingItems;
    [SerializeField] List<GameState> startingGameStates;


    [Header("Don't change these properties")]
    [SerializeField] InventoryList runtimeInventory;

    public Save GetSave() {
        return this.save;
    }

    public List<Item> GetStartingItems() {
        return this.startingItems;
    }

    public List<GameState> GetStartingGameStates() {
        return this.startingGameStates;
    }

    // called before scene transitions
    public void SaveInventory(InventoryList inventory) {
        this.runtimeInventory = inventory;
    }

    public InventoryList GetInventory() {
        return runtimeInventory;
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
