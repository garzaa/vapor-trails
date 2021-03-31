using UnityEngine;

[CreateAssetMenu(fileName = "Scene Container", menuName = "Data Containers/Save Container")]
public class SaveContainer : ScriptableObject {
    public Save save;

    // todo: add a reference to the inventory, items, etc?
}
