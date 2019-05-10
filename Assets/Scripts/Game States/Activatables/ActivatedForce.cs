using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActivatedForce : Activatable {
    public Rigidbody2D target;
    public bool useEntityForward;
    public Vector2 force;

    Entity e;

    void Start() {
        if (target == null) {
            target = GetComponent<Rigidbody2D>();
        }
        if (useEntityForward) {
            e = target.GetComponent<Entity>();
        }
    }

    override public void Activate() {
        Vector2 newForce = force;
        if (useEntityForward) {
            newForce *= e.ForwardVector();
        }
        target.AddForce(newForce);
    }
}