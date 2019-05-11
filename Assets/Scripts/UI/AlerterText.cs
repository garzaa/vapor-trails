using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class AlerterText : MonoBehaviour {
    public GameObject textPrefab;
    public Transform textParent;
    float alertInterval = .08f;
    
    static AlerterText at;

    static Queue<string> alertQueue = new Queue<string>();

    void Start() {
        at = this;
        CheckQueue();
    }

    static void DisplayAlert(string alertText) {
        GameObject g = Instantiate(at.textPrefab, Vector3.zero, Quaternion.identity, at.textParent.transform);
        g.GetComponent<Text>().text = alertText;
        g.transform.SetAsFirstSibling();
    }

    public static void Alert(string alertText) {
        alertQueue.Enqueue(alertText);
    }

    public static void AlertList(string[] alerts) {
        foreach (string a in alerts) {
            alertQueue.Enqueue(a);
        }
    }

    public void CheckQueue() {
        if (alertQueue.Count > 0) {
            DisplayAlert(alertQueue.Dequeue());
        }
        if (at == null) {
            return;
        }
        at.Invoke("CheckQueue", at.alertInterval);
    }
}