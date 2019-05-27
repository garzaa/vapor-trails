using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider2D))]
public class GenericEnterCriteria : Activator {
    public GameObject seek;

    void OnTriggerEnter2d(Collider2D other) {
        if (other.gameObject == seek.gameObject) {
            Activate();
        }
    }
}