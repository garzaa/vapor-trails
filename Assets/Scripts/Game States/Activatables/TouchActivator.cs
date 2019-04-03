using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TouchActivator : MonoBehaviour {
    public Activatable activatable;
    public List<string> wantedTags = new List<string> {
        "Player"
    };

    void OnTriggerEnter2D(Collider2D other) {
        foreach (string s in wantedTags) {
            if (other.gameObject.CompareTag(s)) {
                activatable.Activate();
                return;
            }
        }
    }
}