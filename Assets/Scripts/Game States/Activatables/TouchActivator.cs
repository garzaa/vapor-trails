using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TouchActivator : MonoBehaviour {
    public Activatable activatable;
    public List<string> wantedTags = new List<string> {
        "Player"
    };

    public float cooldown = 2f;
    bool armed = true;

    void OnTriggerEnter2D(Collider2D other) {
        if (!armed) return;
        foreach (string s in wantedTags) {
            if (other.gameObject.CompareTag(s)) {
                activatable.Activate();
                StartCoroutine(CoolDown(cooldown));
                return;
            }
        }
    }

    IEnumerator CoolDown(float seconds) {
        armed = false;
        yield return new WaitForSecondsRealtime(seconds);
        armed = true;
    }
}