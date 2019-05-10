using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RigidBodyStopper : Activatable {
    public Rigidbody2D target;

    Entity e;

    void Start() {
        if (target == null) {
            target = GetComponent<Rigidbody2D>();
        }
    }

    override public void Activate() {
        target.velocity = Vector2.zero;
    }
}