using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityAffector : StateMachineBehaviour {
     
    protected Entity e;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        e = animator.GetComponent<Entity>();
        Enter();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        Update();
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        Exit();
    }

    virtual protected void Enter() {

    }

    virtual protected void Update() {

    }

    virtual protected void Exit() {

    }
}
