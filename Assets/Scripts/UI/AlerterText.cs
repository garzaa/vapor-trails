using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class AlerterText : MonoBehaviour {
    public GameObject textPrefab;
    public Transform textParent;
    float alertInterval = .2f;
    
    static AlerterText at;

    static Queue<string> alertQueue = new Queue<string>();

    void Start() {
        if (at == null) at = this;
        StartCoroutine(CheckQueue());
    }

    static void DisplayAlert(string alertText) {
        GameObject g = Instantiate(at.textPrefab, Vector3.zero, Quaternion.identity, at.textParent.transform);
        g.GetComponent<Text>().text = alertText;
        g.transform.SetAsFirstSibling();
    }

    public static void Alert(Object o) {
        alertQueue.Enqueue(o.ToString());
    }

    public static void Alert(string alertText) {
        alertQueue.Enqueue(alertText);
    }

    public static void AlertList(string[] alerts) {
        foreach (string a in alerts) {
            alertQueue.Enqueue(a);
        }
    }

    public IEnumerator CheckQueue() {
        if (alertQueue.Count > 0) {
            DisplayAlert(alertQueue.Dequeue());
        }
        yield return new WaitForSecondsRealtime(alertInterval);
        at.StartCoroutine(CheckQueue());
    }
}