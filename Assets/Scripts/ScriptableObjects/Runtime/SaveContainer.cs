using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Scene Container", menuName = "Runtime/Save Container")]
public class SaveContainer : ScriptableObject
{
    public Save save;

    // although this loads a blank save, it will just replace it with the new values
    public static SaveContainer GetNewSave() {
        return Resources.Load("ScriptableObjects/Runtime/Saves/New Save") as SaveContainer;
    }

    public bool runtimeLoadedOnce;

    public List<Item> startingItems;

    public void Initialize() {
        runtimeLoadedOnce = true;

    }
}
