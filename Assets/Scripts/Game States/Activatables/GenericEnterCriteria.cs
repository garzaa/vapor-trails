using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider2D))]
public class GenericEnterCriteria : Activator {
    public GameObject seek;
    public List<string> tags = new List<string>();

    void OnTriggerEnter2d(Collider2D other) {
        if (other.gameObject == seek.gameObject || (tags.Count == 0 || tags.Contains(other.gameObject.tag))) {
            Activate();
        }
    }
}