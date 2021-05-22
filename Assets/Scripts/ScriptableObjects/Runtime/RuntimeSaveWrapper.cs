using UnityEngine;

[CreateAssetMenu]
public class RuntimeSaveWrapper : ScriptableObject {
    public Save save;
    public InventoryList inventory;
    public bool loadedOnce;
}
