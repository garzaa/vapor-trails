using UnityEngine;

public class NullParent : MonoBehaviour {
    
    void OnEnable() {
        transform.parent = null;
    }

}