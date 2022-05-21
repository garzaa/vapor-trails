using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostSlide : StateMachineBehaviour
{
    PlayerController pc;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        pc = animator.GetComponent<PlayerController>();
        if(pc.dashCooldown)
        {
            pc.ShortenDashCooldown();
        }
    }
}
