using UnityEngine;
using System.Collections.Generic;

public class RandomChoiceInState : StateMachineBehaviour {

    public List<string> stateNames;

    [Header("Deprecated")]
    public float range;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (stateNames.Count == 0) animator.SetFloat("RandomChoice", Random.Range(0f, range));
    }
}