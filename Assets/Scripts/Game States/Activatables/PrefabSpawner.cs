using UnityEngine;

public class PrefabSpawner : Activatable {
    public GameObject prefab;
    public bool inheritRotation;
    public bool destroyAfterActivation = false;

    override public void ActivateSwitch(bool b) {
        if (b) {
            if (!inheritRotation) {
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