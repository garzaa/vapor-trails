using UnityEngine;
using System.Collections.Generic;

public class DIBehaviour : StateMachineBehaviour {
    public List<Vector2> directions;
    public bool useEntity = true;

    Entity entity;

    Rigidbody2D rigidbody2D;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (useEntity && entity == null) {
            entity = animator.GetComponent<Entity>();
        }
        if (rigidbody2D == null) rigidbody2D = animator.GetComponent<Rigidbody2D>();
        rigidbody2D.velocity = GetRandomDirection();

    }

    Vector2 GetRandomDirection() {
        return directions[Random.Range(0, directions.Count)] * (entity != null ? entity.ForwardVector() : Vector2.one);
    }
}