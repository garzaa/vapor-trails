using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidBodyAffector : StateMachineBehaviour {

    protected Rigidbody2D rb2d;
     
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        rb2d = animator.GetComponent<Rigidbody2D>();
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
