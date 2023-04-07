using UnityEngine;

public class NullParent : MonoBehaviour {
    
    void Start() {
        transform.parent = null;
    }

}
