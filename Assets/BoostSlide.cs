using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostSlide : StateMachineBehaviour
{
    PlayerController pc;
    bool shorterDashCooldown = true;
    
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        pc = animator.GetComponent<PlayerController>();
        if (pc.dashCooldown && shorterDashCooldown)
        {
            pc.ReduceDashCooldown();
        }
    }


    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        pc.StopDashAnimation();
    }
}
