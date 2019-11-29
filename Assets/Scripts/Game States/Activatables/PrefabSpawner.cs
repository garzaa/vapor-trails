using UnityEngine;

public class PrefabSpawner : Activatable {
    public GameObject prefab;
    public bool inheritRoation;
    public bool destroyAfterActivation = false;

    override public void ActivateSwitch(bool b) {
        if (b) {
            if (!inheritRoation) {
                Instantiate(prefab, this.transform.position, Quaternion.identity, null);
            } else {
                Instantiate(prefab, this.transform.position, this.transform.rotation, null);
            }
            if (destroyAfterActivation) {
                Destroy(this.gameObject);
            }
        }
    }
}