using UnityEngine;
using UnityEngine.UI;

public class AlerterText : MonoBehaviour {
    public GameObject textPrefab;
    public Transform textParent;
    
    static AlerterText at;

    void Start() {
        at = this;
        for (int i=0; i<10; i++) {
            Alert("bengis"+i);
        }
    }

    public static void Alert(string alertText) {
        GameObject g = Instantiate(at.textPrefab, Vector3.zero, Quaternion.identity, at.textParent.transform);
        g.GetComponent<Text>().text = alertText;
        g.transform.SetAsFirstSibling();
    }
}