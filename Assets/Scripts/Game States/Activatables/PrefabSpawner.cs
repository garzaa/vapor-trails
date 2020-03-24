using UnityEngine;

public class PrefabSpawner : Activatable {
    public GameObject prefab;
    public bool inheritRotation;
    public bool destroyAfterActivation = false;
    
    public float velocity;

    override public void ActivateSwitch(bool b) {
        if (b) {
            GameObject g;

            if (inheritRotation) {
                g = Instantiate(prefab, this.transform.position, this.transform.rotation, null).gameObject;
            } else {
                g = Instantiate(prefab, this.transform.position, Quaternion.identity, null).gameObject;
            }

            if (g.GetComponent<Rigidbody2D>() != null) {
                g.GetComponent<Rigidbody2D>().velocity = transform.rotation.eulerAngles.normalized * velocity;
            } else if (Mathf.Abs(velocity) > float.Epsilon) {
                Debug.Log("brainlet alert");
            }

            if (destroyAfterActivation) {
                Destroy(this.gameObject);
            }
        }
    }
}